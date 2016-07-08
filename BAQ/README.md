# Sample BAQ

This o365 plugin sample uses custom BAQs (custom queries) which in Epicor ERP 
10.1.500+ will automatically become new REST Resources which you can access at
a url like so:

https://SERVER_NAME/APP_SERVER_NAME/api/v1/BaqSvc/BAQ_NAME

To use this sample as-is you will first need to install the BAQs in this
directory  into your ERP system and also have rest services enabled and 
configured along with the other REST setup prerequisites such as https.