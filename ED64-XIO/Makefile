

ROOTDIR = $(N64_INST)
GCCN64PREFIX = $(ROOTDIR)/bin/mips64-elf-
CHKSUM64PATH = $(ROOTDIR)/bin/chksum64
MKDFSPATH = $(ROOTDIR)/bin/mkdfs
N64TOOL = $(ROOTDIR)/bin/n64tool

FFDIR :=ff
SRCDIR :=src
INCDIR :=inc
RESDIR :=res
OBJDIR = obj
BINDIR = bin

LDDIR :=ldscripts
HDDIR :=headers
#ROMFIX64 =	./tools/romfix64
LINK_FLAGS = -L$(ROOTDIR)/lib -L$(ROOTDIR)/mips64-elf/lib -ldragon -lc -lm -lnosys -T $(LDDIR)/ed64.ld
PROG_NAME =	ED64-XIO-SAMPLE
HEADERNAME = header
HEADERTITLE = $(PROG_NAME)

FLAGS = -std=gnu99 -march=vr4300 -mtune=vr4300 -O2 -Wall -Wno-pointer-sign -I$(ROOTDIR)/mips64-elf/include -Iedlib -I$(FFDIR) -I$(INCDIR) -G0 
FLAGS_VT = -std=gnu99 -march=vr4300 -mtune=vr4300 -O0 -Wall -Wno-pointer-sign -I$(ROOTDIR)/mips64-elf/include -Iedlib -I$(FFDIR) -I$(INCDIR)
ASFLAGS = -mtune=vr4300 -march=vr4300
CC = $(GCCN64PREFIX)gcc
AS = $(GCCN64PREFIX)as
LD = $(GCCN64PREFIX)ld
OBJCOPY = $(GCCN64PREFIX)objcopy

SOURCES = $(wildcard $(SRCDIR)/*.c)
SOURCES_S = $(wildcard $(SRCDIR)/*.s)
SOURCES_FF := $(wildcard $(FFDIR)/*.c)
OBJECTS = $(SOURCES:$(SRCDIR)/%.c=$(OBJDIR)/%.o)
OBJECTS_S = $(SOURCES:$(SRCDIR)/%.s=$(OBJDIR)/%.o)
OBJECTS_FF = $(SOURCES_FF:$(FFDIR)/%.c=$(OBJDIR)/%.o)

$(PROG_NAME).v64: $ $(PROG_NAME).elf
	$(OBJCOPY) $(BINDIR)/$(PROG_NAME).elf $(BINDIR)/$(PROG_NAME).bin -O binary
	rm -f $(BINDIR)/$(PROG_NAME).v64
	$(N64TOOL) -l 256K -t $(HEADERTITLE) -h $(HDDIR)/$(HEADERNAME) -o $(BINDIR)/$(PROG_NAME).v64 $(BINDIR)/$(PROG_NAME).bin
	$(CHKSUM64PATH) $(BINDIR)/$(PROG_NAME).v64

$(PROG_NAME).elf : $(OBJECTS) $(OBJECTS_S) $(OBJECTS_FF)
	$(LD) -o $(BINDIR)/$(PROG_NAME).elf $(OBJECTS) $(OBJECTS_S) $(OBJECTS_FF) $(LINK_FLAGS)

$(OBJECTS): $(OBJDIR)/%.o : $(SRCDIR)/%.c
	$(CC) $(FLAGS) -c $< -o $@

$(OBJECTS_S): $(OBJDIR)/%.o : $(SRCDIR)/%.s
	$(CC) $(ASFLAGS) -s $< -o $@

$(OBJECTS_FF): $(OBJDIR)/%.o : $(FFDIR)/%.c
	$(CC) $(FLAGS) -c $< -o $@


all: $(PROG_NAME).v64

clean:
	$(RM) -f $(BINDIR)/*.v64 
	$(RM) $(BINDIR)/*.elf 
	$(RM) $(OBJDIR)/*.o 
	$(RM)$(BINDIR)/*.bin 
	$(RM)$(BINDIR)/*.dfs