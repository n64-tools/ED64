#include "main.h"

u8 fileRead() {

    u8 *path = "ED64/OS64.v64";
    struct controller_data cd;
    u8 buff[256];
    FIL f;
    UINT br;
    u8 resp;

    screenClear();
    screenRepaint();


    resp = f_open(&f, path, FA_READ);
    if (resp != FR_OK)return resp;

    resp = f_read(&f, buff, sizeof (buff), &br);
    if (resp != FR_OK)return resp;

    resp = f_close(&f);
    if (resp != FR_OK)return resp;


    screenClear();
    screenPrint("Data read from: ");
    screenAppendString(path);
    screenPrint("Press B to exit");

    screenPrint("");
    for (int i = 0; i < sizeof (buff); i++) {
        if (i % 16 == 0)screenPrint("");
        screenAppendHex8(buff[i]);
    }


    screenRepaint();
    for ( ;; ) { //forever
        screenVsync();
        controller_scan();
        cd = get_keys_down();

        if (cd.c[0].B) {
            break;
        }
    }

    return 0;
}

u8 fileWrite() {

    u8 *path = "test.txt";
    u8 *msg = "This is test message";
    struct controller_data cd;
    FIL f;
    UINT bw;
    u8 resp;
    u32 str_len;

    screenClear();
    screenRepaint();

    for (str_len = 0; msg[str_len] != 0; str_len++);

    resp = f_open(&f, path, FA_WRITE | FA_CREATE_ALWAYS);
    if (resp != FR_OK)return resp;

    resp = f_write(&f, msg, str_len, &bw);
    if (resp != FR_OK)return resp;

    resp = f_close(&f);
    if (resp != FR_OK)return resp;


    screenClear();
    screenPrint(msg);
    screenPrint("");
    screenPrint("String above was written to: ");
    screenPrint(path);
    screenPrint("Press B to exit");

    screenRepaint();
    for ( ;; ) { //forever
        screenVsync();
        controller_scan();
        cd = get_keys_down();

        if (cd.c[0].B) {
            break;
        }
    }

    return 0;
}