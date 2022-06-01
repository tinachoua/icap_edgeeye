Q		:=@
CC		:=gcc
SRCS	:=$(wildcard *.c)
TARGET	:=iCAP_AuthHandler
OBJECTS	:=
CFLAGS	:=
LDFLAGS	:=-I/usr/local/include
LDFLAGS	+=-Iinclude
LDFLAGS +=-I../../../../../../../library/libhiredis
LIBS	:=-ljson-c -levent
LIBS	+=../../../../../../../library/libhiredis/libhiredis.a

.PHONY: all
all: $(TARGET)

$(TARGET):
	$(Q)echo "  Building '$@' ..."
	$(Q)$(CC) $(SRCS) -o $@ $(LDFLAGS) $(LIBS) $(CFLAGS)

.PHONY: clean
clean:
	$(Q)echo "  Cleaning '$(TARGET)' ..."
	$(Q)$(RM) $(TARGET)
	$(Q)$(RM) *.o