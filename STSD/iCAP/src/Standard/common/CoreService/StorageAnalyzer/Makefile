Q			=@
SRCDIR  	=
INCLUDEDIR	=include
OBJDIR  	=
BINDIR  	=
TARGET		=iCAP_CoreService_StorAnaly
CXX			?=g++
MACROS		:=
CPPFLAGS	:=-DBOOST_LOG_DYN_LINK $(MACROS)
CXXFLAGS	:=-std=c++11 -Wall -Wextra -O2
CXXFLAGS	+=-I$(INCLUDEDIR)
CXXFLAGS	+=-I../../../../../../../library/liblogger/bin/include
CXXFLAGS	+=-I../libCommon/libmongodb/src/include/
CXXFLAGS	+=-I../libCommon/libMQTTagent/src/include/
CXXFLAGS	+=-I../libCommon/libWebcommand/src/include/
LDFLAGS		:=-L/usr/local/lib
LDFLAGS		+=-L../../../../../../../library/liblogger/bin
LIBS		:=../libCommon/libmongodb/bin/lib_mongodb_64.a
LIBS		+=../libCommon/libMQTTagent/bin/lib_MQTTAgent_64.a
LIBS		+=../libCommon/libWebcommand/bin/lib_WebCommand_64.a
LIBS		+=../../../../../../../library/libcrypto/lib/libcrypto_64.a
LIBS		+=-lbsoncxx -lcrypto -lcurl -lcurlpp -ljsoncpp -lmongocxx -lpaho-mqtt3a -pthread
LIBS		+=-llogger -lboost_date_time -lboost_filesystem -lboost_log -lboost_log_setup -lboost_system -lboost_thread
SOURCES		:=$(wildcard *.cpp)
INCLUDES	:=$(wildcard $(INCLUDEDIR)/*.hpp)
OBJECTS		:=$(SOURCES:%.cpp=%.o)
RM        	:=rm -f

.PHONY: all
all: $(TARGET)

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
	$(Q)$(RM) *.o