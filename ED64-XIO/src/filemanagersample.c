//
// Copyright (c) Krikzz and Contributors.
// See LICENSE file in the project root for full license information.
//

#include "main.h"

u8 fileManager_LoadDirectory(u8 *path, FILINFO *inf, u32 max_items);
u8 fileManager_LoadGame(u8 *path);

#define MAXIMUM_DIRECTORY_COUNT   20
#define MAXIMUM_STRING_LENGTH     36

u8 fileManager() {

    FILINFO inf[MAXIMUM_DIRECTORY_COUNT];
    u32 selector = 0;
    struct controller_data cd;
    u8 resp;

    //open root dir
    resp = fileManager_LoadDirectory("", inf, MAXIMUM_DIRECTORY_COUNT);
    if (resp)return resp;


    for ( ;; ) { //forever

        //print items
        screenClear();
        for (int i = 0; i < MAXIMUM_DIRECTORY_COUNT && inf[i].fname[0]; i++) {
            screenPrint(selector == i ? ">" : " ");
            u8 tmp = inf[i].fname[MAXIMUM_STRING_LENGTH];
            inf[i].fname[MAXIMUM_STRING_LENGTH] = 0; //make sure that the printed string doesn't exceed max len
            screenAppendString(inf[i].fname);
            inf[i].fname[MAXIMUM_STRING_LENGTH] = tmp;
        }

        screenRepaint();

        //controls
        for ( ;; ) { //forever

            screenVsync();
            controller_scan();
            cd = get_keys_down();

            if (cd.c[0].B)return 0;

            if (cd.c[0].up) {
                if (selector != 0)selector--;
                break;
            }

            if (cd.c[0].down) {
                if ((selector + 1) < MAXIMUM_DIRECTORY_COUNT && inf[selector + 1].fname[0])selector++;
                break;
            }

            if (cd.c[0].A && !(inf[selector].fattrib & AM_DIR)) {

                screenClear();
                screenPrint("loading...");
                screenRepaint();

                resp = fileManager_LoadGame(inf[selector].fname);
                if (resp)return resp;

                ed64_set_rom_save_type(ED64_SAVE_TYPE_EEP16K); //set save type
                mainSimulatedRomBoot(CIC_6102); //run the rom
            }
        }
    }

    return 0;
}

u8 fileManager_LoadDirectory(u8 *path, FILINFO *inf, u32 max_items) {

    u8 resp;
    DIR dir;

    resp = f_opendir(&dir, path);
    if (resp)return resp;

    //load directory items
    for (int i = 0; i < max_items; i++) {

        resp = f_readdir(&dir, &inf[i]);
        if (resp)return resp;
        if (inf[i].fname[0] == 0)break; //no directory items anymore
    }

    resp = f_closedir(&dir);
    if (resp)return resp;

    return 0;
}

u8 fileManager_LoadGame(u8 *path) {

    FIL f;
    u8 resp;
    u8 header[8];
    UINT br;
    u32 fsize;

    resp = f_open(&f, path, FA_READ);
    if (resp)return resp;

    fsize = f.obj.objsize - f.fptr;

    //read rom header
    resp = f_read(&f, header, sizeof (header), &br);
    if (resp)return resp;

    //set read position
    resp = f_lseek(&f, 0);
    if (resp)return resp;

    if (header[1] == 0x80) {
        //enable byte swapping for disk operations if rom image has swapped byte order
        //affects only reading to ROM address space
        ed64_rom_write_bytes_swapped(1);
    }

    //warning! file can be read directly to rom but not to bram
    resp = f_read(&f, (void *) ED64_ADDR_ROM, fsize, &br);
    if (resp)return resp;

    ed64_rom_write_bytes_swapped(0);
    if (resp)return resp;

    resp = f_close(&f);
    if (resp)return resp;

    return 0;
}
