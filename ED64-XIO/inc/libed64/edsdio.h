/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

#ifndef ED64_SD_DISKIO_H
#define ED64_SD_DISKIO_H

#ifdef __cplusplus
extern "C" {
#endif

/* Definitions for the SDIO interface. */
#define DISK_ERR_INIT 0xD0 /* command initialization error */
#define DISK_ERR_CTO 0xD1 /* command CTO error */
#define DISK_ERR_RD1 0xD2 /* command timeout */
#define DISK_ERR_RD2 0xD2 /* IO error */
#define DISK_ERR_WR1 0xD3 /* command timeout */
#define DISK_ERR_WR2 0xD3 /* IO error */

unsigned char sd_disk_init();
unsigned char sd_disk_read_to_ram(unsigned long sd_addr, void *dst, unsigned short slen);
unsigned char sd_disk_read_to_rom(unsigned long sd_addr, unsigned long dst, unsigned short slen);
unsigned char sd_disk_read(void *dst, unsigned long saddr, unsigned long slen);
unsigned char sd_disk_write(void *src, unsigned long saddr, unsigned long slen);
unsigned char sd_disk_close_rw();
// unsigned char sd_disk_stop();

#ifdef __cplusplus
}
#endif

#endif /* ED64_SD_DISKIO_H */
