
#include "main.h"

u8 fileRead() {

    u8 *path = "ED64/OS64.v64";
    struct controller_data cd;
    u8 buff[256];
    FIL f;
    UINT br;
    u8 resp;

    graphicsOutputCleanScreen();
    graphicsOutputRepaint();


    resp = f_open(&f, path, FA_READ);
    if (resp)return resp;

    resp = f_read(&f, buff, sizeof (buff), &br);
    if (resp)return resp;

    resp = f_close(&f);
    if (resp)return resp;


    graphicsOutputCleanScreen();
    graphicsOutputPrint("Data read from: ");
    graphicsOutputAppendString(path);
    graphicsOutputPrint("Press B to exit");

    graphicsOutputPrint("");
    for (int i = 0; i < sizeof (buff); i++) {
        if (i % 16 == 0)graphicsOutputPrint("");
        graphicsOutputAppendHex8(buff[i]);
    }


    graphicsOutputRepaint();
    for ( ;; ) { //forever
        graphicsOutputVsync();
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

    graphicsOutputCleanScreen();
    graphicsOutputRepaint();

    for (str_len = 0; msg[str_len] != 0; str_len++);

    resp = f_open(&f, path, FA_WRITE | FA_CREATE_ALWAYS);
    if (resp)return resp;

    resp = f_write(&f, msg, str_len, &bw);
    if (resp)return resp;

    resp = f_close(&f);
    if (resp)return resp;


    graphicsOutputCleanScreen();
    graphicsOutputPrint(msg);
    graphicsOutputPrint("");
    graphicsOutputPrint("String above was written to: ");
    graphicsOutputPrint(path);
    graphicsOutputPrint("Press B to exit");

    graphicsOutputRepaint();
    for ( ;; ) { //forever
        graphicsOutputVsync();
        controller_scan();
        cd = get_keys_down();

        if (cd.c[0].B) {
            break;
        }
    }

    return 0;
}