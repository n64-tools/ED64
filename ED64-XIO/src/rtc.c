
#include "main.h"
#include "sys.h"
#include <string.h> /* required for memcpy */

static volatile struct SI_regs_s * const SI_regs = (struct SI_regs_s *) 0xa4800000; /* could also be defined in sys.c */
static void * const PIF_RAM = (void *) 0x1fc007c0; /* could also be defined in sys.c */
/** @brief SI DMA busy */
#define SI_STATUS_DMA_BUSY  ( 1 << 0 )
/** @brief SI IO busy */
#define SI_STATUS_IO_BUSY   ( 1 << 1 )

static void __SI_DMA_wait(void) {
    while (SI_regs->status & (SI_STATUS_DMA_BUSY | SI_STATUS_IO_BUSY));
}

static void __controller_exec_PIF(void *inblock, void *outblock) {
    volatile uint64_t inblock_temp[8];
    volatile uint64_t outblock_temp[8];

    data_cache_hit_writeback_invalidate(inblock_temp, 64);
    memcpy(UncachedAddr(inblock_temp), inblock, 64);

    /* Be sure another thread doesn't get into a resource fight */
    disable_interrupts();

    __SI_DMA_wait();

    SI_regs->DRAM_addr = inblock_temp; /* only cares about 23:0 */
    MEMORY_BARRIER();
    SI_regs->PIF_addr_write = PIF_RAM; /* is it really ever anything else? */
    MEMORY_BARRIER();

    __SI_DMA_wait();

    data_cache_hit_writeback_invalidate(outblock_temp, 64);

    SI_regs->DRAM_addr = outblock_temp;
    MEMORY_BARRIER();
    SI_regs->PIF_addr_read = PIF_RAM;
    MEMORY_BARRIER();

    __SI_DMA_wait();

    /* Now that we've copied, its safe to let other threads go */
    enable_interrupts();

    memcpy(outblock, UncachedAddr(outblock_temp), 64);
}

int rtc_status() {
    static unsigned long long SI_eeprom_status_block[8] = {
        0x00000000ff010306,
        0xfffffffffe000000,
        0,
        0,
        0,
        0,
        0,
        1
    };
    static unsigned long long output[8];

    __controller_exec_PIF(SI_eeprom_status_block, output);

    /* We are looking for 0x80 in the second byte returned, which
     * signifies that there is an EEPROM present.*/
    return output[1] >> 32;

}


void rtc_read(unsigned char block, unsigned char *data) {

    static unsigned long long SI_eeprom_read_block[8] = {
        0x0000000002080400, /* LSB is block */
        0xffffffffffffffff, /* return data will be this quad */
        0xfffe000000000000,
        0,
        0,
        0,
        0,
        1
    };
    static unsigned long long output[8];

    SI_eeprom_read_block[0] = 0x0000000002090700 | block;
    __controller_exec_PIF(SI_eeprom_read_block, output);

    memcpy(&output[1], data, 9);


}


void display_rtc_current_dt() {

    unsigned long rtc_stat; // __attribute__((unused));
    unsigned char g_rtc_data[9];

    unsigned char sec = 0;
    unsigned char min = 0;
    unsigned char hou = 0;
    unsigned char day_ow __attribute__((unused)) = 0;
    unsigned char day_om = 0;
    unsigned char year = 0;
    unsigned char month = 0;

    rtc_stat = rtc_status() >> 8;
    rtc_read(2, g_rtc_data);


    g_rtc_data[2] &= 0x7f;
    g_rtc_data[5] &= 0x7f;


    sec = g_rtc_data[0];
    min = g_rtc_data[1];
    hou = g_rtc_data[2] & 0x7f;
    day_om = g_rtc_data[3];
    day_ow = g_rtc_data[4];
    month = g_rtc_data[5] & 0x7f;
    year = g_rtc_data[6];

    screen_print("rtc status: ");
    screen_append_hex32_print(rtc_stat);

    screen_print("rtc_rd block 2: ");
    for (int i = 0; i < 9; i++) {
        screen_append_hex8_print(g_rtc_data[i]);
    }
    screen_print("Current RTC (ISO 8601) timestamp: ");

    screen_print("    20");
    screen_append_hex8_print(year);
    screen_append_str_print("-");
    screen_append_hex8_print(month);
    screen_append_str_print("-");
    screen_append_hex8_print(day_om);
    screen_append_str_print("T");
    screen_append_hex8_print(hou);
    screen_append_str_print(":");
    screen_append_hex8_print(min);
    screen_append_str_print(":");
    screen_append_hex8_print(sec);

}


void menu_display_rtc() {
    struct controller_data cd;
    unsigned char data[128];

    for (;;) {

        /* print items */
        screen_clear();
        screen_print("Real Time Clock");
        screen_print("Press (B) to exit");
        screen_print("");

        unsigned long id = ed64_bios_get_cart_id();
        switch (id) {
            case ED64_CART_ID_V3:
            case ED64_CART_ID_X7:
                //TODO: Should ensure EEPROM save is off and RTC is on!
                screen_print("NOTE: this will only work if the ROM was loaded with RTC=ON"); //-extra=Rtc
                //(and Flash/Sra Ram Savetype?
                screen_print("");
                display_rtc_current_dt();
                break;
            default:
                screen_append_str_print("RTC unavailable on this cart type!");
                break;
        }


//#if LIBDRAGON_SYS
        controller_scan();
        cd = get_keys_down();
        if (cd.c[0].B)return;
//#else
//#endif
        screen_repaint();
        screen_vsync();
    }
}
