#ifndef __MAIN_H__
#define __MAIN_H__
#pragma pack(1) 
typedef struct{
    char Header[5];
    char Command[2];
    short Length;
    char CheckSum[5];
    char payload[50];
}PACKAGE;
#pragma pack(0)

#endif
