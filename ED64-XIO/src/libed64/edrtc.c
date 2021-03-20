

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

// void ed_i2c_write_bit(unsigned char val) {

//     val &= 1;
//     ed_bios_reg_wr(val);
//     ed_bios_reg_wr(4 | val);
//     ed_bios_reg_wr(val);
// }

// void ed_i2c_write_byte(unsigned char val) {

//     u32 i;
//     unsigned char bit;

//     for (i = 0; i < 8; i++) {
//         bit = val >> (7 - i);
//         bit &= 1;
//         ed_i2c_write_bit(bit);
//     }

//     ed_bios_reg_wr(1);
//     ed_bios_reg_wr(5);
//     ed_bios_reg_wr(1);
// }

// unsigned char ed_i2c_read_byte() {

//     u32 i;
//     unsigned char val = 0;

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

// unsigned char ed_i2c_read_byte(unsigned char addr) {

//     unsigned char val;
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

// unsigned char ed_i2c_write_byte(unsigned char addr, unsigned char val) {

//     ed_i2c_start();
//     ed_i2c_write_byte(0xD0);
//     ed_i2c_write_byte(addr);
//     ed_i2c_write_byte(val);
//     ed_i2c_stop();

//     return val;
// }

// void ed_rtc_set_date(u16 year, unsigned char month, unsigned char date, unsigned char day) {

//     ed_i2c_write_byte(3, day);

//     ed_i2c_write_byte(4, date);
//     ed_i2c_write_byte(5, month);

//     year %= 100;
//     ed_i2c_write_byte(6, year);

// }

// void ed_rtc_set_time(unsigned char hours, unsigned char mins, unsigned char sec) {

//     ed_i2c_write_byte(0, sec);
//     ed_i2c_write_byte(1, mins);
//     ed_i2c_write_byte(2, hours);
// }

// void ed_rtc_set_datetime(u16 year, unsigned char month, unsigned char day_of_mo, unsigned char day_of_we, unsigned char hours, unsigned char mins, unsigned char sec) {

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

// void ed_rtc_set_datetime_asread(unsigned char *rtc_data) {

//     u16 i;
//     for (i = 0; i < 16; i++) ed_i2c_write_byte(i, rtc_data[i]);
// }

// void ed_rtc_get_datetime(unsigned char *rtc_data) {

//     unsigned char i;
//     for (i = 0; i < 7; i++)rtc_data[i] = ed_i2c_read_byte(i);

// }

// unsigned char ed_rtc_read_hex(unsigned char reg) {
//     unsigned char val;
//     val = ed_i2c_read_byte(reg);
//     if (reg == 2 || reg == 5)val &= 0x7f;
//     val = (val & 0x0f) + (val >> 4) * 10;
//     return val;
// }

// unsigned char ed_rtc_get_hour() {
//     return ed_rtc_read_hex(2);
// }

// unsigned char ed_rtc_get_min() {

//     return ed_rtc_read_hex(1);
// }

// unsigned char ed_rtc_get_sec() {

//     return ed_rtc_read_hex(0);
// }

// unsigned char ed_rtc_get_dayofweek() {

//     return ed_rtc_read_hex(3);
// }

// unsigned char ed_rtc_get_dayofmonth() {

//     return ed_rtc_read_hex(4);
// }

// unsigned char ed_rtc_get_month() {

//     return ed_rtc_read_hex(5);
// }

// unsigned char ed_rtc_get_year() {

//     return ed_rtc_read_hex(6);
// }

// unsigned char *days[] = {
//     "???",
//     "Monday",
//     "Tuesday",
//     "Wednesday",
//     "Thursday",
//     "Friday",
//     "Saturday",
//     "Sunday"
// };

