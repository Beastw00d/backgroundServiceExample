# backgroundServiceExample

An example application which uses an IHostedService/BackgroundService to periodically check if the local cache needs to be updated from events that have been journaled.

The Billing.API service will manage invoices for customers. Each invoice will include the customer name which can be changed through the Customer.API. The Customer.API will journal the CustomerNameChangedEvent and the Billing.API will have a background task to look at the journalDB periodically to see if any new events have come in and if so it will update the customer name on the saved invoices.