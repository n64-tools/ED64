//
// Copyright (c) Krikzz and Contributors.
// See LICENSE file in the project root for full license information.
//

#include "main.h"

u8 fmLoadDir(u8 *path, FILINFO *inf, u32 max_items);
u8 fmLoadGame(u8 *path);

#define MAX_DIRECTORY_COUNT   20
#define MAX_STRING_LENGTH     36

u8 fmanager() {

    FILINFO inf[MAX_DIRECTORY_COUNT];
    u32 selector = 0;
    struct controller_data cd;
    u8 resp;

    //open root dir
    resp = fmLoadDir("", inf, MAX_DIRECTORY_COUNT);
    if (resp)return resp;


    for ( ;; ) { //forever

        //print items
        gCleanScreen();
        for (int i = 0; i < MAX_DIRECTORY_COUNT && inf[i].fname[0]; i++) {
            gConsPrint(selector == i ? ">" : " ");
            u8 tmp = inf[i].fname[MAX_STRING_LENGTH];
            inf[i].fname[MAX_STRING_LENGTH] = 0; //make sure that the printed string doesn't exceed max len
            gAppendString(inf[i].fname);
            inf[i].fname[MAX_STRING_LENGTH] = tmp;
        }

        gRepaint();

        //controls
        for ( ;; ) { //forever

            gVsync();
            controller_scan();
            cd = get_keys_down();

            if (cd.c[0].B)return 0;

            if (cd.c[0].up) {
                if (selector != 0)selector--;
                break;
            }

            if (cd.c[0].down) {
                if ((selector + 1) < MAX_DIRECTORY_COUNT && inf[selector + 1].fname[0])selector++;
                break;
            }

            if (cd.c[0].A && !(inf[selector].fattrib & AM_DIR)) {

                gCleanScreen();
                gConsPrint("loading...");
                gRepaint();

                resp = fmLoadGame(inf[selector].fname);
                if (resp)return resp;

                ed64SetRomSaveType(SAVE_TYPE_EEP16K); //set save type
                mainSimulatedRomBoot(CIC_6102); //run the rom
            }
        }
    }

    return 0;
}

u8 fmLoadDir(u8 *path, FILINFO *inf, u32 max_items) {

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

u8 fmLoadGame(u8 *path) {

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
        ed64RomWriteByteswap(1);
    }

    //warning! file can be read directly to rom but not to bram
    resp = f_read(&f, (void *) ED64_ADDR_ROM, fsize, &br);
    if (resp)return resp;

    ed64RomWriteByteswap(0);
    if (resp)return resp;

    resp = f_close(&f);
    if (resp)return resp;

    return 0;
}
