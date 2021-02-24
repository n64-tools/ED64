//
// Copyright (c) Krikzz and Contributors.
// See LICENSE file in the project root for full license information.
//

#ifndef __MAIN_H
#define	__MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

#include "screen.h"
#include "ed64.h"
#include "sddisk.h"
#include "ff.h"

void perform_simulated_rom_boot(u8 cic);
u8 fileManager();
void usbTerminal();
void usbLoadRom();
u8 fileRead();
u8 fileWrite();

#ifdef __cplusplus
}
#endif

#endif	/* MAIN_H */
