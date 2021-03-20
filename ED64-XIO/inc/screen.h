/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

#ifndef SCREEN_H
#define SCREEN_H

#ifdef __cplusplus
extern "C" {
#endif

#include "everdrive.h"
#include "stdlib.h"
#include "string.h"

#define TV_SCREEN_W 40 /* screen width (horizontal) */
#define TV_SCREEN_H 30 /* screen height (vertical) */
#define G_BORDER_X 2
#define G_BORDER_Y 2
#define G_MAX_STR_LEN (TV_SCREEN_W - G_BORDER_X * 2)

#define REGION_PAL_B1 0x1000
#define REGION_PAL_B2 0x2000
#define REGION_PAL_B3 0x3000
#define REGION_PAL_G1 0x1400
#define REGION_PAL_G2 0x2400
#define REGION_PAL_G3 0x3400

#define REGION_PAL_BR 0x5000
#define REGION_PAL_BG 0x6000
#define REGION_PAL_WG 0x1700
#define REGION_PAL_RB 0x0500

void screen_clear();
void screen_print(u8 *str);
void screen_set_xy_pos(u8 x, u8 y);
void screen_set_pal(u16 pal);
void screen_append_str_print(u8 *str);
void screen_append_char_print(u8 chr);
void screen_append_hex8_print(u8 val);
void screen_append_hex16_print(u16 val);
void screen_append_hex32_print(u32 val);
void screen_append_hex32_print(u32 val);
void screen_repaint();
void screen_vsync();

#ifdef __cplusplus
}
#endif

#endif /* SCREEN_H */