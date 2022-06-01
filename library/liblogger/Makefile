Q			=@
SRCDIR  	=src
INCLUDEDIR	=$(SRCDIR)/include
OBJDIR  	=obj
BINDIR  	=bin
TARGET		=$(OBJDIR)/liblogger.a
CXX			?=g++
MACROS		:=
CPPFLAGS	:=-DBOOST_LOG_DYN_LINK $(MACROS)
CXXFLAGS	:=-std=c++11 -Wall -Wextra -ggdb -I$(INCLUDEDIR)
SOURCES		:=$(wildcard $(SRCDIR)/*.cpp)
INCLUDES	:=$(wildcard $(INCLUDEDIR)/*.hpp)
OBJECTS		:=$(SOURCES:$(SRCDIR)/%.cpp=$(OBJDIR)/%.o)
RM        	:=rm -r -f

.PHONEY: all
all: clean gendir $(TARGET) install

$(TARGET): $(OBJECTS)
	$(Q)echo "  Building '$@' ..."
	$(Q)ar rcs -o $@ $(OBJECTS)

$(OBJECTS): $(OBJDIR)/%.o : $(SRCDIR)/%.cpp
	$(Q)$(CXX) $(CPPFLAGS) $(CXXFLAGS) -o $@ -c $<
	$(Q)echo "  Compiled "$<" successfully!"

.PHONEY: gendir
gendir:
	$(Q)mkdir -p $(OBJDIR)

.PHONY: install
install:
	$(Q)mkdir -p $(BINDIR)/include
	$(Q)cp $(TARGET) $(BINDIR) 
	$(Q)cp $(INCLUDES) $(BINDIR)/include

.PHONEY: clean
clean:
	$(Q)$(RM) $(OBJDIR)
	$(Q)$(RM) $(BINDIR)
	$(Q)echo "  Cleanup complete!"
