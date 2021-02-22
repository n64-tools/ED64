//
// Copyright (c) Krikzz and Contributors.
// See LICENSE file in the project root for full license information.
//

#include "graphics.h"

u16 *g_disp_ptr;
u16 g_cur_pal;
u16 g_cons_ptr;
u8 g_last_x;
u8 g_last_y;
u16 gfx_buff[G_SCREEN_W * G_SCREEN_H];

void gDrawChar8X8(u32 val, u32 x, u32 y) {

    u64 tmp;
    u32 font_val;
    u8 *font_ptr = &font[(val & 0xff) * 8];
    u64 *ptr = (u64 *) & screen.current[ (x * 8 + y * 8 * screen.pixel_w)];
    u64 pal_bgr = pal[(val >> 8) & 0x0f];
    u64 pal_txt = pal[val >> 12];

    for (int i = 0; i < 4; i++) {
        pal_bgr = (pal_bgr << 16) | pal_bgr;
    }

    for (u32 i = 0; i < 8; i++) {

        font_val = *font_ptr++;

        tmp = pal_bgr;
        if ((font_val & 0x80) != 0)tmp = (tmp & 0x0000FFFFFFFFFFFF) | (pal_txt << 48);
        if ((font_val & 0x40) != 0)tmp = (tmp & 0xFFFF0000FFFFFFFF) | (pal_txt << 32);
        if ((font_val & 0x20) != 0)tmp = (tmp & 0xFFFFFFFF0000FFFF) | (pal_txt << 16);
        if ((font_val & 0x10) != 0)tmp = (tmp & 0xFFFFFFFFFFFF0000) | (pal_txt);
        *ptr++ = tmp;

        tmp = pal_bgr;
        if ((font_val & 0x08) != 0)tmp = (tmp & 0x0000FFFFFFFFFFFF) | (pal_txt << 48);
        if ((font_val & 0x04) != 0)tmp = (tmp & 0xFFFF0000FFFFFFFF) | (pal_txt << 32);
        if ((font_val & 0x02) != 0)tmp = (tmp & 0xFFFFFFFF0000FFFF) | (pal_txt << 16);
        if ((font_val & 0x01) != 0)tmp = (tmp & 0xFFFFFFFFFFFF0000) | (pal_txt);
        *ptr++ = tmp;

        ptr += (screen.pixel_w - 8) / 4;
    }
}

void gRepaint() {

    u16 *chr_ptr = gfx_buff;

    screen.buff_sw = (screen.buff_sw ^ 1) & 1;
    screen.current = screen.buff[screen.buff_sw];


    for (u32 y = 0; y < screen.h; y++) {
        for (u32 x = 0; x < screen.w; x++) {

            gDrawChar8X8(*chr_ptr++, x, y);
        }
    }

    data_cache_hit_writeback(screen.current, screen.buff_len * 2);
    gVsync();
    vregs[1] = (vu32) screen.current;

}

void gVsync() {

    while (vregs[4] == 0x200);
    while (vregs[4] != 0x200);
}


void gAppendHex4(u8 val);

void gCleanScreen() {

    g_cur_pal = 0;
    gSetXY(G_BORDER_X, G_BORDER_Y);
    for (int i = 0; i < G_SCREEN_W * G_SCREEN_H; i++)gfx_buff[i] = PAL_B3;
    gSetPal(PAL_B1);
}

void gSetPal(u16 pal) {
    g_cur_pal = pal;
}

void gAppendString(u8 *str) {
    while (*str != 0)*g_disp_ptr++ = *str++ + g_cur_pal;
}

void gAppendChar(u8 chr) {

    *g_disp_ptr++ = chr + g_cur_pal;
}

void gAppendHex4(u8 val) {

    val += (val < 10 ? '0' : '7');
    *g_disp_ptr++ = val + g_cur_pal;
}

void gAppendHex8(u8 val) {

    gAppendHex4(val >> 4);
    gAppendHex4(val & 15);
}

void gAppendHex16(u16 val) {

    gAppendHex8(val >> 8);
    gAppendHex8(val);
}

void gAppendHex32(u32 val) {

    gAppendHex16(val >> 16);
    gAppendHex16(val);

}

void gSetXY(u8 x, u8 y) {

    g_cons_ptr = x + y * G_SCREEN_W;
    g_disp_ptr = &gfx_buff[g_cons_ptr];
    g_last_x = x;
    g_last_y = y;
}

void gConsPrint(u8 *str) {

    g_disp_ptr = &gfx_buff[g_cons_ptr];
    g_cons_ptr += G_SCREEN_W;
    g_last_y++;
    gAppendString(str);
}