//
// Copyright (c) Krikzz and Contributors.
// See LICENSE file in the project root for full license information.
//

#ifndef __ED64_MAIN_H
#define	__ED64_MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

#include "sys.h"
#include "ed64.h"
#include "disk.h"
#include "ff.h"

void mainBootSimulator(u8 cic);
u8 fmanager();
void usbTerminal();
void usbLoadGame();
u8 fileRead();
u8 fileWrite();

#ifdef __cplusplus
}
#endif

#endif	/* MAIN_H */
