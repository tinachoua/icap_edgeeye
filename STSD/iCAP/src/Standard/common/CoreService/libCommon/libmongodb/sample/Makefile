Q			=@
SRCDIR  	=../src
INCLUDEDIR	=$(SRCDIR)/include
OBJDIR  	=../obj
BINDIR  	=../bin
TARGET		=sample
CXX			?=g++
MACROS		:=
CPPFLAGS	:=$(MACROS)
CXXFLAGS	:=-std=c++11 -Wall -Wextra
CXXFLAGS	+=-I$(INCLUDEDIR)
LDFLAGS		:=-L../../../../../../../../../library/liblogger/bin
LIBS		:=$(OBJDIR)/lib_mongodb_64.a
LIBS		+=-lbsoncxx -lmongocxx -ljsoncpp -pthread
LIBS		+=-llogger -lboost_date_time -lboost_filesystem -lboost_log -lboost_log_setup -lboost_system -lboost_thread
SOURCES		:=$(wildcard *.cpp)
INCLUDES	:=$(wildcard $(INCLUDEDIR)/*.hpp)
OBJECTS		:=$(SOURCES:%.cpp=%.o)
RM        	:=rm -f

.PHONY: all
all: clean $(TARGET)

$(TARGET): $(OBJECTS)
	$(Q)$(CXX) $^ -o $@ $(LDFLAGS) $(LIBS)
	$(Q)echo "  Linking '$@' complete!"

$(OBJECTS): %.o : %.cpp
	$(Q)$(CXX) $(CPPFLAGS) $(CXXFLAGS) -o $@ -c $<
	$(Q)echo "  Compiled '$<' successfully."

.PHONY: clean
clean:
	$(Q)echo "  Cleaning '$(TARGET)' ..."
	$(Q)$(RM) $(TARGET)
	$(Q)$(RM) $(OBJECTS)