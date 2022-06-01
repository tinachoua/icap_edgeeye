#!/usr/bin/python
import sys, os, tarfile

#main
try:
	version = 'RELEASE'
	if len(sys.argv) > 1:
		version = sys.argv[1]

	os.rename("iCAP_Server_Release","iCAP_Server_linux_V_1_6_1")
		
	output_folder = 'iCAP_Server_linux_V_1_6_1/'
	print('Start packing file...')
	with tarfile.open('iCAP_Server_linux_V_1_6_1.tar.gz', 'w:gz') as tf:
		print(output_folder + 'eula.md...');
		tf.add(output_folder + 'eula.md')
		print(output_folder + 'icap_admindb.tar...');
		tf.add(output_folder + 'icap_admindb.tar')
		print(output_folder + 'icap_datadb.tar...');
		tf.add(output_folder + 'icap_datadb.tar')
		print(output_folder + 'icap_redis.tar...');
		tf.add(output_folder + 'icap_redis.tar')
		print(output_folder + 'icap_gateway.tar...');
		tf.add(output_folder + 'icap_gateway.tar')
		if version == 'DEMOKIT':
			print(output_folder + 'icap_mockdata.tar...')
			tf.add(output_folder + 'icap_mockdata.tar')
		# print(output_folder + 'icap_cluster_manager.tar...');
		# tf.add(output_folder + 'icap_cluster_manager.tar')
		print(output_folder + 'innoAGE-Gateway.tar...');
		tf.add(output_folder + 'innoAGE-Gateway.tar')
		print(output_folder + 'innoAGE-WebService.tar...');
		tf.add(output_folder + 'innoAGE-WebService.tar')
		print(output_folder + 'icap_coreservice_dm.tar...');
		tf.add(output_folder + 'icap_coreservice_dm.tar')
		print(output_folder + 'icap_coreservice_datahandler.tar...');
		tf.add(output_folder + 'icap_coreservice_datahandler.tar')
		print(output_folder + 'icap_coreservice_notify.tar...');
		tf.add(output_folder + 'icap_coreservice_notify.tar')
		print(output_folder + 'icap_coreservice_storanalyzer.tar...');
		tf.add(output_folder + 'icap_coreservice_storanalyzer.tar')
		print(output_folder + 'icap_coreservice_dashboardagent.tar...');
		tf.add(output_folder + 'icap_coreservice_dashboardagent.tar')
		print(output_folder + 'icap_coreservice_dlm.tar...');
		tf.add(output_folder + 'icap_coreservice_dlm.tar')
		print(output_folder + 'icap_coreservice_innoagemanager.tar...');
		tf.add(output_folder + 'icap_coreservice_innoagemanager.tar')
		print(output_folder + 'icap_dbchecker.tar...');
		tf.add(output_folder + 'icap_dbchecker.tar')
		print(output_folder + 'iCAP_Server_Installer...');
		tf.add(output_folder + 'iCAP_Server_Installer')
		print(output_folder + 'iCAP_Server_Uninstall.sh...');
		tf.add(output_folder + 'iCAP_Server_Uninstall.sh')
		print(output_folder + 'icap_webservice_authapi.tar...');
		tf.add(output_folder + 'icap_webservice_authapi.tar')
		print(output_folder + 'icap_webservice_dashboardapi.tar...');
		tf.add(output_folder + 'icap_webservice_dashboardapi.tar')
		print(output_folder + 'icap_webservice_deviceapi.tar...');
		tf.add(output_folder + 'icap_webservice_deviceapi.tar')
		print(output_folder + 'icap_webservice_website.tar...');
		tf.add(output_folder + 'icap_webservice_website.tar')
		print(output_folder + 'Images.tar.gz...');
		tf.add(output_folder + 'Images.tar.gz')
		print(output_folder + 'icap_webservice_oobservice.tar...');
		tf.add(output_folder + 'icap_webservice_oobservice.tar')
		print(output_folder + 'setting.json...');
		tf.add(output_folder + 'setting.json')
		print(output_folder + 'setting.env...');
		tf.add(output_folder + 'setting.env')
		print(output_folder + 'Log_Update.sh...');
		tf.add(output_folder + 'Log_Update.sh')
	print('Finished.')

except KeyboardInterrupt:
	print("")
	sys.exit()