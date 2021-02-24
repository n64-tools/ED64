/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

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
u8 file_menu_display();
void usb_terminal_display();
void usb_terminal_load_rom();
u8 file_display_read();
u8 file_display_write();

#ifdef __cplusplus
}
#endif

#endif	/* MAIN_H */
