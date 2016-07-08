# Epicor o365 Outlook Addin

This repository contains a small sample built by Microsoft for creating an o365
Outlook Addin which gets data from Epicor ERP for the customer associated with
a customer contact email. 

## Functional Overview

This is a simple outlook panel which shows some information about a customer
from Epicor ERP when an email from someone at that company is selected. Outlook
uses the ERP 10.1.500+ rest services to query for customer contacts with the
email and then shows information like sales history and credit status on the
customer.

## Installation & Setup

Detail Upcoming, but basics are:

* Ensure that REST Servicing is set up with ERP 10.1.500
* Install the BAQs included (coming soon) in this project
* Configure the service to point to your ERP server's rest services
* ...? deployment steps for the outlook plugin bit.

