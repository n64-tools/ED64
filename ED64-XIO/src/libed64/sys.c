/*
* Copyright (c) Krikzz and Contributors.
* See LICENSE file in the project root for full license information.
*/

#include "sys.h"

const unsigned long ntsc_320[] = {
    0x00013002, 0x00000000, 0x00000140, 0x00000200,
    0x00000000, 0x03e52239, 0x0000020d, 0x00000c15,
    0x0c150c15, 0x006c02ec, 0x002501ff, 0x000e0204,
    0x00000200, 0x00000400
};


const unsigned long mpal_320[] = {
    0x00013002, 0x00000000, 0x00000140, 0x00000200,
    0x00000000, 0x04651e39, 0x0000020d, 0x00040c11,
    0x0c190c1a, 0x006c02ec, 0x002501ff, 0x000e0204,
    0x00000200, 0x00000400
};

const unsigned long pal_320[] = {
    0x00013002, 0x00000000, 0x00000140, 0x00000200,
    0x00000000, 0x0404233a, 0x00000271, 0x00150c69,
    0x0c6f0c6e, 0x00800300, 0x005f0239, 0x0009026b,
    0x00000200, 0x00000400
};

#define SI_STATUS_DMA_BUSY  ( 1 << 0 )
#define SI_STATUS_IO_BUSY   ( 1 << 1 )
#define SYS_MAX_PIXEL_W   320
#define SYS_MAX_PIXEL_H   240

void sys_n64_region_init();
void sysPI_rd_safe(void *ram, unsigned long pi_address, unsigned long len);
void sysPI_wr_safe(void *ram, unsigned long pi_address, unsigned long len);


static volatile struct PI_regs_s * const PI_regs = (struct PI_regs_s *) 0xa4600000;
static volatile struct SI_regs_s * const SI_regs = (struct SI_regs_s *) 0xa4800000; //Un-used in sample
//static void * const PIF_RAM = (void *) 0x1fc007c0; //Un-used in sample
static volatile unsigned long *vregs = (volatile unsigned long *) 0xa4400000;
BootStrap *sys_boot_strap = (BootStrap *) 0x80000300;

Screen screen;

void sys_n64_init() {
//#if LIBDRAGON_SYS
    disable_interrupts();
    set_AI_interrupt(0);
    set_VI_interrupt(0, 0);
    set_PI_interrupt(0);
    set_DP_interrupt(0);
//#else
//#endif

    IO_WRITE(PI_STATUS_REG, 3);
    IO_WRITE(PI_BSD_DOM1_LAT_REG, 0x40);
    IO_WRITE(PI_BSD_DOM1_PWD_REG, 0x12);
    IO_WRITE(PI_BSD_DOM1_PGS_REG, 0x07);
    IO_WRITE(PI_BSD_DOM1_RLS_REG, 0x03);



    IO_WRITE(PI_BSD_DOM2_LAT_REG, 0x05);
    IO_WRITE(PI_BSD_DOM2_PWD_REG, 0x0C);
    IO_WRITE(PI_BSD_DOM2_PGS_REG, 0x0D);
    IO_WRITE(PI_BSD_DOM2_RLS_REG, 0x02);
    /*
    IO_WRITE(PI_BSD_DOM2_LAT_REG, 0x40);
    IO_WRITE(PI_BSD_DOM2_PWD_REG, 0x12);
    IO_WRITE(PI_BSD_DOM2_PGS_REG, 0x07);
    IO_WRITE(PI_BSD_DOM2_RLS_REG, 0x03);*/

//#if LIBDRAGON_SYS
    rdp_init();
//#else
//#endif

    screen.buff_sw = 0;
    screen.buff[0] = (unsigned short *) malloc(SYS_MAX_PIXEL_W * SYS_MAX_PIXEL_H * 2);
    screen.buff[1] = (unsigned short *) malloc(SYS_MAX_PIXEL_W * SYS_MAX_PIXEL_H * 2);
    screen.current = screen.buff[screen.buff_sw];
    screen.bgr_ptr = 0;

    sys_n64_region_init();

}

void sys_n64_region_init() {

    unsigned long i;
    unsigned long *v_setup;
    unsigned long region = *(unsigned long *) 0x80000300;

//#if LIBDRAGON_SYS
    disable_interrupts();
//#else
//#endif


    screen.w = 40;
    screen.h = 30;
    screen.pixel_w = 320;
    screen.pixel_h = 240;
    screen.buff_len = screen.pixel_w * screen.pixel_h;
    screen.char_h = 8;
    v_setup = region == REGION_NTSC ? (unsigned long*) ntsc_320 : region == REGION_PAL ? (unsigned long*) pal_320 : (unsigned long*) mpal_320;

    while (IO_READ(VI_CURRENT_REG) > 10);
    IO_WRITE(VI_CONTROL_REG, 0);

    vregs[1] = (volatile unsigned long) screen.current;
    vregs[2] = v_setup[2];
    for (i = 5; i < 9; i++)vregs[i] = v_setup[i];
    vregs[9] = v_setup[9]; //bnack
    vregs[10] = v_setup[10];
    vregs[11] = v_setup[11];
    vregs[3] = v_setup[3];
    vregs[12] = v_setup[12];
    vregs[13] = v_setup[13];
    vregs[0] = v_setup[0];

//#if LIBDRAGON_SYS
    enable_interrupts();
//#else
//#endif
}

void sys_n64_pi_read(void *ram, unsigned long pi_address, unsigned long len) {

    pi_address &= 0x1FFFFFFF;

//#if LIBDRAGON_SYS
    data_cache_hit_writeback_invalidate(ram, len);
    disable_interrupts();
//#else
//#endif

//#if LIBDRAGON_SYS
    while (dma_busy());
//#else
//#endif
    IO_WRITE(PI_STATUS_REG, 3);
    PI_regs->ram_address = ram;
    PI_regs->pi_address = pi_address; //(pi_address | 0x10000000) & 0x1FFFFFFF;
    PI_regs->write_length = len - 1;
//#if LIBDRAGON_SYS
    while (dma_busy());

    enable_interrupts();
//#else
//#endif
}

void sys_n64_pi_write(void *ram, unsigned long pi_address, unsigned long len) {

    pi_address &= 0x1FFFFFFF;

//#if LIBDRAGON_SYS
    data_cache_hit_writeback(ram, len);
    disable_interrupts();

    while (dma_busy());
//#else
//#endif
    IO_WRITE(PI_STATUS_REG, 3);
    PI_regs->ram_address = ram;
    PI_regs->pi_address = pi_address; //(pi_address | 0x10000000) & 0x1FFFFFFF;
    PI_regs->read_length = len - 1;

//#if LIBDRAGON_SYS
    while (dma_busy());

    enable_interrupts();
//#else
//#endif
}
