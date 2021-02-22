//
// Copyright (c) Krikzz and Contributors.
// See LICENSE file in the project root for full license information.
//

#ifndef __ED64_GRAPHICS_H
#define	__ED64_GRAPHICS_H

#ifdef __cplusplus
extern "C" {
#endif

#include "types.h"
#include "everdrive.h"

#define G_SCREEN_W      40 //screen.w
#define G_SCREEN_H      30 //screen.h
#define G_BORDER_X      2
#define G_BORDER_Y      2
// #define G_MAX_STR_LEN   (G_SCREEN_W - G_BORDER_X*2)

#define PAL_B1          0x1000
// #define PAL_B2          0x2000
#define PAL_B3          0x3000
// #define PAL_G1          0x1400
// #define PAL_G2          0x2400
// #define PAL_G3          0x3400

// #define PAL_BR          0x5000
#define PAL_BG          0x6000
// #define PAL_WG          0x1700
// #define PAL_RB          0x0500

void gCleanScreen();
void gConsPrint(u8 *str);
void gSetXY(u8 x, u8 y);
void gSetPal(u16 pal);
void gAppendString(u8 *str);
void gAppendChar(u8 chr);
void gAppendHex8(u8 val);
void gAppendHex16(u16 val);
void gAppendHex32(u32 val);
void gAppendHex32(u32 val);
void gRepaint();
void gVsync();

#ifdef __cplusplus
}
#endif

#endif	/* GRAPHICS_H */