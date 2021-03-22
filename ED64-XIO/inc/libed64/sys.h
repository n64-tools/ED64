/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

#ifndef SYS_H
#define SYS_H

#ifdef __cplusplus
extern "C" {
#endif

#include <libdragon.h>

#define CIC_6101 1
#define CIC_6102 2
#define CIC_6103 3
#define CIC_5101 4
#define CIC_6105 5
#define CIC_6106 6
#define CIC_5167 7

#define REGION_PAL 0
#define REGION_NTSC 1
#define REGION_MPAL 2

#define KSEG0 0x80000000
#define KSEG1 0xA0000000

#define VI_BASE_REG 0x04400000
#define VI_CONTROL (VI_BASE_REG + 0x00)
#define VI_FRAMEBUFFER (VI_BASE_REG + 0x04)
#define VI_WIDTH (VI_BASE_REG + 0x08)
#define VI_V_INT (VI_BASE_REG + 0x0C)
#define VI_CUR_LINE (VI_BASE_REG + 0x10)
#define VI_TIMING (VI_BASE_REG + 0x14)
#define VI_V_SYNC (VI_BASE_REG + 0x18)
#define VI_H_SYNC (VI_BASE_REG + 0x1C)
#define VI_H_SYNC2 (VI_BASE_REG + 0x20)
#define VI_H_LIMITS (VI_BASE_REG + 0x24)
#define VI_COLOR_BURST (VI_BASE_REG + 0x28)
#define VI_H_SCALE (VI_BASE_REG + 0x2C)
#define VI_VSCALE (VI_BASE_REG + 0x30)

#define PI_BSD_DOM1_LAT_REG (PI_BASE_REG + 0x14)
#define PI_BSD_DOM1_PWD_REG (PI_BASE_REG + 0x18)
#define PI_BSD_DOM1_PGS_REG (PI_BASE_REG + 0x1C) /*   page size */
#define PI_BSD_DOM1_RLS_REG (PI_BASE_REG + 0x20)

#define PI_BSD_DOM2_LAT_REG (PI_BASE_REG + 0x24) /* Domain 2 latency */
#define PI_BSD_DOM2_PWD_REG (PI_BASE_REG + 0x28) /*   pulse width */
#define PI_BSD_DOM2_PGS_REG (PI_BASE_REG + 0x2C) /*   page size */
#define PI_BSD_DOM2_RLS_REG (PI_BASE_REG + 0x30) /*   release duration */

#define PHYS_TO_K1(x) ((unsigned long)(x) | KSEG1) /* physical to kseg1 */
#define IO_WRITE(addr, data) (*(volatile unsigned long *)PHYS_TO_K1(addr) = (unsigned long)(data))
#define IO_READ(addr) (*(volatile unsigned long *)PHYS_TO_K1(addr))

#define PI_STATUS_REG (PI_BASE_REG + 0x10)
#define PI_BASE_REG 0x04600000

#define VI_BASE_REG 0x04400000
#define VI_STATUS_REG (VI_BASE_REG + 0x00)
#define VI_CONTROL_REG VI_STATUS_REG
#define VI_CURRENT_REG (VI_BASE_REG + 0x10)

#define RGB(r, g, b) ((r << 11) | (g << 6) | (b << 1))

typedef struct BootStrap {
  volatile unsigned long region;
  volatile unsigned long rom_type;
  volatile unsigned long base_addr;
  volatile unsigned long rst_type;
  volatile unsigned long cic_type;
  volatile unsigned long os_ver;
  volatile unsigned long ram_size;
  volatile unsigned long app_buff;
} BootStrap;

typedef struct Screen {
  unsigned long w;
  unsigned long h;
  unsigned long pixel_w;
  unsigned long pixel_h;
  unsigned long char_h;
  unsigned long buff_len;
  unsigned long buff_sw;
  unsigned short *buff[2];
  unsigned short *current;
  unsigned short *bgr_ptr;
} Screen;

// typedef struct PI_regs_s {
//     /** @brief Uncached address in RAM where data should be found */
//     void * ram_address;
//     /** @brief Address of data on peripheral */
//     unsigned long pi_address;
//     /** @brief How much data to read from RAM into the peripheral */
//     unsigned long read_length;
//     /** @brief How much data to write to RAM from the peripheral */
//     unsigned long write_length;
//     /** @brief Status of the PI, including DMA busy */
//     unsigned long status;
// } PI_regs_s;

// typedef struct SI_regs_s {
//     /** @brief Uncached address in RAM where data should be found */
//     volatile void * DRAM_addr;
//     /** @brief Address to read when copying from PIF RAM */
//     volatile void * PIF_addr_read;
//     /** @brief Reserved word */
//     uint32_t reserved1;
//     /** @brief Reserved word */
//     uint32_t reserved2;
//     /** @brief Address to write when copying to PIF RAM */
//     volatile void * PIF_addr_write;
//     /** @brief Reserved word */
//     uint32_t reserved3;
//     /** @brief SI status, including DMA busy and IO busy */
//     uint32_t status;
// } SI_regs_t;

void sys_n64_init();
void sys_n64_pi_read(void *ram, unsigned long pi_address, unsigned long len);
void sys_n64_pi_write(void *ram, unsigned long pi_address, unsigned long len);


#ifdef __cplusplus
}
#endif

#endif /* SYS_H */
