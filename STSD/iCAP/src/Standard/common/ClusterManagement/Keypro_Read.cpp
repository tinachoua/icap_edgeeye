#include <iostream>
#include <algorithm>
#include <sstream>
#include <stdio.h>
#include <string.h>
#include <fstream>
#include <string>

#include "hasp_api_cpp.h"
#include "vendor_code.h"
#include "errorprinter.h"
#include "Crypto.hpp"

using namespace std;

#define CUSTOM_FEATURE 42

const size_t totalDataLan = 355;
unsigned char data[256];
hasp_size_t datalen=sizeof(data);

/***************************************************************************/

//Displays the content of mdata in a nice format with an ascii
//and a hexadecimal area.
void displayMemory(unsigned char *mdata, unsigned int mdataLen)
{
	//The number of bytes to be shown in each line
	const unsigned int lineLen = 16;

	//This is printed before the first char in each line
	const char* margin = "     ";

	unsigned int i/*lines*/, ii/*chars*/;

	for (i = 0; i < mdataLen; i += lineLen)
	{
		ostringstream asciiWriter;
		ostringstream hexWriter;

		for (ii = i; ii < lineLen + i; ii++)
		{
			if (ii < mdataLen)
			{
				//Print the active char to the hex view
				unsigned int val = static_cast<unsigned int>(mdata[ii]);
				if (val < 16)
					hexWriter << '0';
				hexWriter << uppercase << hex << val << ' ';

				//... and to the ascii view
				if (mdata[ii] < 32 || mdata[ii]>127) //Do not print some characters
					asciiWriter << '.';             //because this may create
				else                              //problems on UNIX terminals
					asciiWriter << mdata[ii];
			}
			else
			{
				hexWriter << "   ";
				asciiWriter << " ";
			}

			//Display a separator after each 4th byte
			if (!((ii - i + 1) % 4) && (ii - i + 1 < lineLen))
				hexWriter << "| ";
		}

		//Write the line
		cout << margin << hexWriter.str() << " [" << asciiWriter.str() << "]" << endl;
	}

	cout << endl;
}

void write_to_file(unsigned char * decrypted, int decrypted_length) {
    FILE *stream;   
    stream = fopen("./license.dat", "wb+");
    if (stream == NULL) {
        fprintf (stderr, "ERROR: Cannot open license.dat\n");
        return;
    }
    fwrite(decrypted, 1, decrypted_length, stream);
    fclose(stream);
}

int check_keypro_status()
{
	ErrorPrinter	errorPrinter;
	haspStatus		status;
    
	/**************Login******************/
	Chasp hasp(ChaspFeature::defaultFeature());
	status = hasp.login(vendorCode);
	
	if (!HASP_SUCCEEDED(status))
	{
		cout << "login from default feature : ";
		errorPrinter.printError(status);
		cout << endl;
		return -1;
	}

	/****************Logout****************/
	status = hasp.logout();

	if (!HASP_SUCCEEDED(status))
	{
		cout << "logout from default feature : ";
		errorPrinter.printError(status);
		cout << endl;
		return -1;
	}

	return 0;
}

int Keypro_readlicense(void)
{
	ErrorPrinter	errorPrinter;
	haspStatus		status;

	/************************************************************************
	 * hasp_login
	 *   establishes a context for HASP services
	 */
	Chasp hasp(ChaspFeature::defaultFeature());
	status = hasp.login(vendorCode);
	
	if (!HASP_SUCCEEDED(status))
	{
		printf("login to default feature : ");
		errorPrinter.printError(status);
		exit(-1);
	}

	//Reads the memory
	hasp_size_t size = 0;
	ChaspFile mainFile = hasp.getFile(ChaspFile::fileReadWrite);

	status = mainFile.getFileSize(size);

	if (!HASP_SUCCEEDED(status))
	{
		cout << "Sentinel key memory size is " << size << " bytes : ";
		errorPrinter.printError(status);
		status = hasp.logout();
		exit(-1);
	}

	unsigned char read[totalDataLan] = { 0 };
	status = mainFile.read(read, totalDataLan);

	if (!HASP_SUCCEEDED(status)) 
	{
		cout << "read key : ";
		errorPrinter.printError(status);
		//displayMemory(read, totalDataLan);
		status = hasp.logout();
		exit(-1);
	}
	
	status = hasp.decrypt(read, totalDataLan);

	if (!HASP_SUCCEEDED(status)) 
	{
		cout << "decrypt : ";
		errorPrinter.printError(status);
		//displayMemory(read, totalDataLan);
		status = hasp.logout();
		exit(-1);
	}

    /************************************************************************
     * hasp_logout
     *   closes established session and releases allocated memory
     */
	status = hasp.logout();

	if (!HASP_SUCCEEDED(status))
	{
		cout << "logout from default feature : ";
		errorPrinter.printError(status);
        exit(-1);
    }

	Crypto crypto;
	unsigned char *plaintext = NULL;
	int len = 64;

	plaintext = crypto.rsa_decrypt(read, &len);
	if (plaintext == NULL) {
		cout << "rsa decrypt failed" << endl;
		exit(-1);
	}

#ifdef DEBUG
	cout << "Decrypted string : " << plaintext << endl;
#endif
 
    write_to_file(plaintext, strlen((const char*)plaintext));
#ifdef DEBUG
	cout << "It's OK to generate the license.dat." << endl;
#endif

	if (plaintext) {
		free(plaintext);
		plaintext = NULL;
	}

    return 0;

}