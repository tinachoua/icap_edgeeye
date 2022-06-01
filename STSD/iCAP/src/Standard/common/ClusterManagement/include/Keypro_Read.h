#ifndef __KEYPRO_READ_H__
#define __KEYPRO_READ_H__

int Keypro_readlicense();
int check_keypro_status();
void write_to_file(unsigned char * decrypted, int decrypted_length);

#endif
