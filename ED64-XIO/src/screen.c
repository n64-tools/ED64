/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

#include "screen.h"

/****************************************************************************** 
 * screen graphics
******************************************************************************/
u16 *screen_display_ptr;
u16 screen_cur_pal;
u16 screen_cons_ptr;
u8 screen_last_x;
u8 screen_last_y;
u16 screen_buffer[TV_SCREEN_W * TV_SCREEN_H];

extern u8 font[];

void screen_draw_char_8X8(u32 val, u32 x, u32 y) {

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

void screen_repaint() {

    u16 *chr_ptr = screen_buffer;

    screen.buff_sw = (screen.buff_sw ^ 1) & 1;
    screen.current = screen.buff[screen.buff_sw];


    for (u32 y = 0; y < screen.h; y++) {
        for (u32 x = 0; x < screen.w; x++) {

            screen_draw_char_8X8(*chr_ptr++, x, y);
        }
    }

    data_cache_hit_writeback(screen.current, screen.buff_len * 2);
    screen_vsync();
    vregs[1] = (vu32) screen.current;

}

void screen_vsync() {

    while (vregs[4] == 0x200);
    while (vregs[4] != 0x200);
}


void screen_append_hex4_print(u8 val);

void screen_clear() {

    screen_cur_pal = 0;
    screen_set_xy_pos(G_BORDER_X, G_BORDER_Y);
    for (int i = 0; i < TV_SCREEN_W * TV_SCREEN_H; i++)screen_buffer[i] = REGION_PAL_B3;
    screen_set_pal(REGION_PAL_B1);
}

void screen_set_pal(u16 pal) {
    screen_cur_pal = pal;
}

void screen_append_str_print(u8 *str) {
    while (*str != 0)*screen_display_ptr++ = *str++ + screen_cur_pal;
}

void screen_append_char_print(u8 chr) {

    *screen_display_ptr++ = chr + screen_cur_pal;
}

void screen_append_hex4_print(u8 val) {

    val += (val < 10 ? '0' : '7');
    *screen_display_ptr++ = val + screen_cur_pal;
}

void screen_append_hex8_print(u8 val) {

    screen_append_hex4_print(val >> 4);
    screen_append_hex4_print(val & 15);
}

void screen_append_hex16_print(u16 val) {

    screen_append_hex8_print(val >> 8);
    screen_append_hex8_print(val);
}

void screen_append_hex32_print(u32 val) {

    screen_append_hex16_print(val >> 16);
    screen_append_hex16_print(val);

}

void screen_set_xy_pos(u8 x, u8 y) {

    screen_cons_ptr = x + y * TV_SCREEN_W;
    screen_display_ptr = &screen_buffer[screen_cons_ptr];
    screen_last_x = x;
    screen_last_y = y;
}

void screen_print(u8 *str) {

    screen_display_ptr = &screen_buffer[screen_cons_ptr];
    screen_cons_ptr += TV_SCREEN_W;
    screen_last_y++;
    screen_append_str_print(str);
}
