/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

#ifndef ED_ROM_SAMPLE_H
#define ED_ROM_SAMPLE_H

#ifdef __cplusplus
extern "C" {
#endif

#include "everdrive.h"
#include "screen.h"


void rom_boot_simulator(unsigned char cic);
unsigned char fmanager_display();
void usb_terminal();
void usb_load_rom();
unsigned char fm_file_read();
unsigned char fm_file_write();
void menu_display_rtc();

#ifdef __cplusplus
}
#endif

#endif /* ED_ROM_SAMPLE_H */
