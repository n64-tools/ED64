

// /* */

// void ed_i2c_start() {
//     ed_bios_reg_wr(5);
//     ed_bios_reg_wr(4);
//     ed_bios_reg_wr(0);
// }

// void ed_i2c_stop() {
//     ed_bios_reg_wr(4);
//     ed_bios_reg_wr(5);
// }

// void ed_i2c_write_bit(u8 val) {

//     val &= 1;
//     ed_bios_reg_wr(val);
//     ed_bios_reg_wr(4 | val);
//     ed_bios_reg_wr(val);
// }

// void ed_i2c_write_byte(u8 val) {

//     u32 i;
//     u8 bit;

//     for (i = 0; i < 8; i++) {
//         bit = val >> (7 - i);
//         bit &= 1;
//         ed_i2c_write_bit(bit);
//     }

//     ed_bios_reg_wr(1);
//     ed_bios_reg_wr(5);
//     ed_bios_reg_wr(1);
// }

// u8 ed_i2c_read_byte() {

//     u32 i;
//     u8 val = 0;

//     for (i = 0; i < 8; i++) {

//         ed_bios_reg_wr(1);
//         ed_bios_reg_wr(5);
//         val = (val << 1) | (ed_bios_reg_read() & 1);
//         ed_bios_reg_wr(1);
//     }

//     ed_bios_reg_wr(1);
//     ed_bios_reg_wr(5);
//     ed_bios_reg_wr(1);

//     return val;
// }

// u8 ed_i2c_read_byte(u8 addr) {

//     u8 val;
//     ed_i2c_start();
//     ed_i2c_write_byte(0xD0);
//     ed_i2c_write_byte(addr);
//     ed_i2c_stop();

//     ed_i2c_start();
//     ed_i2c_write_byte(0xD1);
//     val = ed_i2c_read_byte();

//     ed_i2c_stop();

//     return val;
// }

// u8 ed_i2c_write_byte(u8 addr, u8 val) {

//     ed_i2c_start();
//     ed_i2c_write_byte(0xD0);
//     ed_i2c_write_byte(addr);
//     ed_i2c_write_byte(val);
//     ed_i2c_stop();

//     return val;
// }

// void ed_rtc_set_date(u16 year, u8 month, u8 date, u8 day) {

//     ed_i2c_write_byte(3, day);

//     ed_i2c_write_byte(4, date);
//     ed_i2c_write_byte(5, month);

//     year %= 100;
//     ed_i2c_write_byte(6, year);

// }

// void ed_rtc_set_time(u8 hours, u8 mins, u8 sec) {

//     ed_i2c_write_byte(0, sec);
//     ed_i2c_write_byte(1, mins);
//     ed_i2c_write_byte(2, hours);
// }

// void ed_rtc_set_datetime(u16 year, u8 month, u8 day_of_mo, u8 day_of_we, u8 hours, u8 mins, u8 sec) {

//     u16 i;
//     for (i = 7; i < 16; i++)ed_i2c_write_byte(i, 0);

//     year %= 100;

//     ed_i2c_write_byte(0, sec);
//     ed_i2c_write_byte(1, mins);
//     ed_i2c_write_byte(2, hours);
//     ed_i2c_write_byte(3, day_of_we);
//     ed_i2c_write_byte(4, day_of_mo);
//     ed_i2c_write_byte(5, month);
//     ed_i2c_write_byte(6, year);
// }

// void ed_rtc_set_datetime_asread(u8 *rtc_data) {

//     u16 i;
//     for (i = 0; i < 16; i++) ed_i2c_write_byte(i, rtc_data[i]);
// }

// void ed_rtc_get_datetime(u8 *rtc_data) {

//     u8 i;
//     for (i = 0; i < 7; i++)rtc_data[i] = ed_i2c_read_byte(i);

// }

// u8 ed_rtc_read_hex(u8 reg) {
//     u8 val;
//     val = ed_i2c_read_byte(reg);
//     if (reg == 2 || reg == 5)val &= 0x7f;
//     val = (val & 0x0f) + (val >> 4) * 10;
//     return val;
// }

// u8 ed_rtc_get_hour() {
//     return ed_rtc_read_hex(2);
// }

// u8 ed_rtc_get_min() {

//     return ed_rtc_read_hex(1);
// }

// u8 ed_rtc_get_sec() {

//     return ed_rtc_read_hex(0);
// }

// u8 ed_rtc_get_dayofweek() {

//     return ed_rtc_read_hex(3);
// }

// u8 ed_rtc_get_dayofmonth() {

//     return ed_rtc_read_hex(4);
// }

// u8 ed_rtc_get_month() {

//     return ed_rtc_read_hex(5);
// }

// u8 ed_rtc_get_year() {

//     return ed_rtc_read_hex(6);
// }

// u8 *days[] = {
//     "???",
//     "Monday",
//     "Tuesday",
//     "Wednesday",
//     "Thursday",
//     "Friday",
//     "Saturday",
//     "Sunday"
// };

