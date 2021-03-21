/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

#include "screen.h"
#include "string.h"

/****************************************************************************** 
 * screen graphics
******************************************************************************/
unsigned short *screen_display_ptr;
unsigned short screen_cur_pal;
unsigned short screen_cons_ptr;
unsigned char screen_last_x;
unsigned char screen_last_y;
unsigned short screen_buffer[TV_SCREEN_W * TV_SCREEN_H];

extern unsigned char font[];
extern Screen screen;  /* this is also defined in sys.c */
static volatile unsigned long *vregs = (volatile unsigned long *) 0xa4400000; /* this is also defined in sys.c */

unsigned short pal[16] = {
    RGB(0, 0, 0), RGB(31, 31, 31), RGB(16, 16, 16), RGB(28, 28, 2),
    RGB(8, 8, 8), RGB(31, 0, 0), RGB(0, 31, 0), RGB(12, 12, 12),
    0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0aa0
};

void screen_draw_char_8X8(unsigned long val, unsigned long x, unsigned long y) {

    unsigned long long tmp;
    unsigned long font_val;
    unsigned char *font_ptr = &font[(val & 0xff) * 8];
    unsigned long long *ptr = (unsigned long long *) & screen.current[ (x * 8 + y * 8 * screen.pixel_w)];
    unsigned long long pal_bgr = pal[(val >> 8) & 0x0f];
    unsigned long long pal_txt = pal[val >> 12];

    for (int i = 0; i < 4; i++) {
        pal_bgr = (pal_bgr << 16) | pal_bgr;
    }

    for (unsigned long i = 0; i < 8; i++) {

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

    unsigned short *chr_ptr = screen_buffer;

    screen.buff_sw = (screen.buff_sw ^ 1) & 1;
    screen.current = screen.buff[screen.buff_sw];


    for (unsigned long y = 0; y < screen.h; y++) {
        for (unsigned long x = 0; x < screen.w; x++) {

            screen_draw_char_8X8(*chr_ptr++, x, y);
        }
    }

    data_cache_hit_writeback(screen.current, screen.buff_len * 2);
    screen_vsync();
    vregs[1] = (volatile unsigned long) screen.current;

}

void screen_vsync() {

    while (vregs[4] == 0x200);
    while (vregs[4] != 0x200);
}


void screen_append_hex4_print(unsigned char val);

void screen_clear() {

    screen_cur_pal = 0;
    screen_set_xy_pos(G_BORDER_X, G_BORDER_Y);
    for (int i = 0; i < TV_SCREEN_W * TV_SCREEN_H; i++)screen_buffer[i] = REGION_PAL_B3;
    screen_set_pal(REGION_PAL_B1);
}

void screen_set_pal(unsigned short pal) {
    screen_cur_pal = pal;
}

void screen_append_str_print(unsigned char *str) {
    while (*str != 0)*screen_display_ptr++ = *str++ + screen_cur_pal;
}

void screen_append_char_print(unsigned char chr) {

    *screen_display_ptr++ = chr + screen_cur_pal;
}

void screen_append_hex4_print(unsigned char val) {

    val += (val < 10 ? '0' : '7');
    *screen_display_ptr++ = val + screen_cur_pal;
}

void screen_append_hex8_print(unsigned char val) {

    screen_append_hex4_print(val >> 4);
    screen_append_hex4_print(val & 15);
}

void screen_append_hex16_print(unsigned short val) {

    screen_append_hex8_print(val >> 8);
    screen_append_hex8_print(val);
}

void screen_append_hex32_print(unsigned long val) {

    screen_append_hex16_print(val >> 16);
    screen_append_hex16_print(val);

}

void screen_set_xy_pos(unsigned char x, unsigned char y) {

    screen_cons_ptr = x + y * TV_SCREEN_W;
    screen_display_ptr = &screen_buffer[screen_cons_ptr];
    screen_last_x = x;
    screen_last_y = y;
}

void screen_print(unsigned char *str) {

    screen_display_ptr = &screen_buffer[screen_cons_ptr];
    screen_cons_ptr += TV_SCREEN_W;
    screen_last_y++;
    screen_append_str_print(str);
}
