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
#include "disk.h"
#include "ff.h"

void mainSimulatedRomBoot(u8 cic);
u8 fmanager();
void usbTerminal();
void usbLoadRom();
u8 fileRead();
u8 fileWrite();

#ifdef __cplusplus
}
#endif

#endif	/* MAIN_H */
