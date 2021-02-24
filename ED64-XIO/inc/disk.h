//
// Copyright (c) Krikzz and Contributors.
// See LICENSE file in the project root for full license information.
//

#ifndef __ED64_DISK_H
#define	__ED64_DISK_H

#ifdef __cplusplus
extern "C" {
#endif

#define DISK_ERR_INIT   0xD0
#define DISK_ERR_CTO    0xD1
#define DISK_ERR_RD1    0xD2//cmd tout
#define DISK_ERR_RD2    0xD2//io error
#define DISK_ERR_WR1    0xD3//cmd tout
#define DISK_ERR_WR2    0xD3//io error

u8 diskInit();
u8 sd_disk_read_to_ram(u32 sd_addr, void *dst, u16 slen);
u8 sd_disk_read_to_rom(u32 sd_addr, u32 dst, u16 slen);
u8 sd_disk_read(void *dst, u32 saddr, u32 slen);
u8 disk_write(void *src, u32 saddr, u32 slen);
u8 disk_close_rw();
u8 disk_stop();

#ifdef __cplusplus
}
#endif

#endif	/* DISK_H */
