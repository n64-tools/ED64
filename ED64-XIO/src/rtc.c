
#include "main.h"
#include "regsinternal.h"

static volatile struct SI_regs_s * const SI_regs = (struct SI_regs_s *) 0xa4800000;
static void * const PIF_RAM = (void *) 0x1fc007c0;
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

    SI_regs->DRAM_addr = inblock_temp; // only cares about 23:0
    MEMORY_BARRIER();
    SI_regs->PIF_addr_write = PIF_RAM; // is it really ever anything else?
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
        0x0000000002080400, // LSB is block
        0xffffffffffffffff, // return data will be this quad
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

void menu_display_rtc() {
    struct controller_data cd;
    unsigned char data[128];
    // unsigned char w = 18;
    // unsigned char h = 10;
    // u16 y;
    // u16 x = (g_screen_w - w) / 2 + 1;
    // unsigned char min = 0;
    // unsigned char sec = 0;
    // unsigned char hour = 0;
    // unsigned char day_ow = 0;
    // unsigned char day_om = 0;
    // unsigned char year = 0;
    // unsigned char month = 0;
    // unsigned char rtc_data[9];
    // unsigned char rtc_edit = 0;
    // unsigned char key_zero = 0;
    // unsigned char selector = 0;
    // unsigned char sub_sel = 0;

    //ed_bios_reg_mode_io(); //set i2c reg?

    for (;;) {

        /* print items */
        screen_clear();
        screen_print("Real Time Clock");
        screen_print("Press (B) to exit");
        screen_repaint();

        screen_print("rtc status: ");
        screen_append_hex32_print(rtc_status());

        screen_print("rtc_rd: ");
        rtc_read(2, data);
        for (int i = 0; i < 9; i++) {
            screen_append_hex8_print(data[i]);
        }

        screen_print("rtc_rd: ");
        rtc_read(0, data);
        for (int i = 0; i < 9; i++) {
            screen_append_hex8_print(data[i]);
        }

        screen_vsync();
//#if LIBDRAGON_SYS
        controller_scan();
        cd = get_keys_down();
        if (cd.c[0].B)return;
//#else
//#endif
        screen_repaint();
    }

        // if (!rtc_edit) {

        //     ed_rtc_get_datetime(rtc_data);
        //     sec = rtc_data[0];
        //     min = rtc_data[1];
        //     hour = rtc_data[2] & 0x7f;
        //     day_om = rtc_data[4];
        //     day_ow = rtc_data[3];
        //     month = rtc_data[5] & 0x7f;
        //     year = rtc_data[6];

        //     if (day_ow > 7)day_ow = 1;
        //     sub_sel = 0;
        //     selector = 0;
        // }

        //screen_repaint();

        //for (;;) {
            // screen_clear();
            // screen_repaint();
            // screen_print("RTC Setup ", x, y);

            // //screen_set_color(COLOR_FILE);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);

            // y += 2;
            // screen_print("Date: ", x, y);

            // if (selector == 0 && sub_sel == 0 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            // screen_append_hex8_print(0x20);
            // screen_append_hex8_print(year);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);
            // screen_append_str_print("-");
            // if (selector == 0 && sub_sel == 1 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            // screen_append_hex8_print(month);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);
            // screen_append_str_print("-");
            // if (selector == 0 && sub_sel == 2 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            // screen_append_hex8_print(day_om);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);


            // y += 2;
            // screen_print("Time: ", x, y);
            // if (selector == 1 && sub_sel == 0 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            // screen_append_hex8_print(hour);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);
            // screen_append_str_print(":");
            // if (selector == 1 && sub_sel == 1 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            // screen_append_hex8_print(min);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);
            // screen_append_str_print(":");
            // if (selector == 1 && sub_sel == 2 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            // screen_append_hex8_print(sec);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);

            // y += 2;
            // screen_print("Day : ", x, y);
            // if (selector == 2 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            // screen_append_str_print(days[day_ow]);
            // //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);

            // y += 2;
            // if (rtc_edit) {
            //     if (selector == 3 && rtc_edit)//screen_set_color(COLOR_SELECTED);
            //     screen_print_cx("Apply", y);
            //     //if (rtc_edit)//screen_set_color(COLOR_MENU_ITEM);
            // }


            // screen_repaint();
            // if (!rtc_edit)controller_scan();
            // if (rtc_edit)get_keys_down();

            // if (joy == 0)key_zero = 1;

            // if ((cd.c[0].A) && !rtc_edit && key_zero) {
            //     rtc_edit = 1;
            //     sec = 0;
            //     // ed_bios_reg_off();
            //     continue;
            // }

            // if ((cd.c[0].B) && !rtc_edit && key_zero) {
            //     return;
            // }
            // if (!rtc_edit)continue;

            // if ((cd.c[0].A) && selector == 0) {
            //     if (sub_sel == 0) {
            //         year++;
            //         if ((year & 0x0f) > 9)year = (year & 0xf0) + 0x10;
            //         if (year > 0x60)year = 0;
            //     }

            //     if (sub_sel == 1) {
            //         month++;
            //         if ((month & 0x0f) > 9)month = (month & 0xf0) + 0x10;
            //         if (month > 0x12 || month == 0)month = 1;
            //     }

            //     if (sub_sel == 2) {
            //         day_om++;
            //         if ((day_om & 0x0f) > 9)day_om = (day_om & 0xf0) + 0x10;
            //         if (day_om > 0x31 || day_om == 0)day_om = 1;
            //     }

            // }

            // if ((cd.c[0].A) && selector == 1) {
            //     if (sub_sel == 0) {
            //         hour++;
            //         if ((hour & 0x0f) > 9)hour = (hour & 0xf0) + 0x10;
            //         if (hour > 0x23)hour = 0;
            //     }

            //     if (sub_sel == 1) {
            //         min++;
            //         if ((min & 0x0f) > 9)min = (min & 0xf0) + 0x10;
            //         if (min > 0x59)min = 0;
            //     }

            //     if (sub_sel == 2) {
            //         sec++;
            //         if ((sec & 0x0f) > 9)sec = (sec & 0xf0) + 0x10;
            //         if (sec > 0x59)sec = 0;
            //     }
            // }

            // if ((cd.c[0].A) && selector == 2) {
            //     day_ow = day_ow >= 7 ? 1 : day_ow + 1;

            // }

            // if ((cd.c[0].A) && selector == 3) {


            //     ed_rtc_set_datetime(year, month, day_om, day_ow, hour, min, sec);
            //     // while (controller_scan() != 0);
            //     rtc_edit = 0;
            //     key_zero = 0;
            // }

            // if (cd.c[0].B) {
            //     rtc_edit = 0;
            //     key_zero = 0;
            // }

            // if (cd.c[0].up) {
            //     selector = selector == 0 ? 3 : selector - 1;
            //     sub_sel = 0;
            // }

            // if (cd.c[0].down) {
            //     selector = selector == 3 ? 0 : selector + 1;
            //     sub_sel = 0;
            // }

            // if (cd.c[0].left) {
            //     sub_sel = sub_sel == 0 ? 2 : sub_sel - 1;
            // }

            // if (cd.c[0].right) {
            //     sub_sel = sub_sel == 2 ? 0 : sub_sel + 1;
            // }
        //}
    //}
}
