//The MIT License (MIT)

//Copyright (c) Microsoft Corporation

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

/// <reference path="C:\Source\Repos\SandBox-RC-O365\Partner-PoC\Epicor\E10OutlookConnectorv1\E10OutlookConnectorWeb\Scripts/FabricUI/toast.js" />

(function () {
    "use strict";

    // object used to transport the credentials, token and context from the add-in to the Epicor API
    var request;

    // The Office initialize function must be run each time a new page is loaded.
    Office.initialize = function (reason) {
        $(document).ready(function () {

            // register the login button click
            $('#login').click(login);

            request = new Object();
            request.userName = "";
            request.password = "";
            request.context = "";
            request.token = "";

            // try and load values associated with the mail item's context
            loadvalues();
        });
    };

    function loadvalues() {

        // the request token has 2 roles...
        // 1. is the unique identifier of this mailbox
        // 2. allows retrieval of the cached credentials (if any)
        if (request.token == "") {
            Office.context.mailbox.getUserIdentityTokenAsync(serviceCallback);
        } else {
            // make the call to the Epicor API
            makeServiceRequest();
        }
    }

    // if credentials are required, provides the functionality when the user clicks the 
    // login button.
    function login() {
        var userName = document.getElementById("username");
        var password = document.getElementById("password");

        var logonForm = document.getElementById("logonForm");
        logonForm.setAttribute("class", "hiddenPage");
        var profile = document.getElementById("profile");
        profile.setAttribute("class", "displayedPage");

        request.userName = userName.value;

        // the password has not been protected / encyrpted - please change if required.
        request.password = password.value;

        // retry the call to the Epicor API
        makeServiceRequest();
    }

    // Function called when the request for an identity token is complete.
    function serviceCallback(asyncResult, userContext) {
        // Cache the token from the Exchange server.
        request.token = asyncResult.value;
        // Make a request to the service.
        makeServiceRequest();
    };

    function makeServiceRequest() {

        //// get the context - in this the email address
        var item = Office.context.mailbox.item;
        request.context = item.sender.emailAddress;
        var chartYTDData = new Array();

        var chartXAxis = new Array();
        var chartYAxis = new Array();

        $.ajax({
            url: '../../api/e10/contact',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json;charset=utf-8'
        }).done(function (data) {

            var customer = data.value[0];

            // apply the data from the API call to the elements within the HTML page.

            $("#contact").text(customer.CustCnt_Name);
            $("#title").text(customer.CustCnt_ContactTitle);
            $("#company").text(customer.Company_Name + ' (' + customer.Customer_Company + ')');
            $("a#email").attr('href', 'mailto:' + customer.CustCnt_EMailAddress);
            $("a#email").text(customer.CustCnt_EMailAddress);
            $("a#telephone").attr('href', 'skype:' + customer.CustCnt_PhoneNum + '?call');
            $("a#telephone").text(customer.CustCnt_PhoneNum);
            $("a#cell").attr('href', 'skype:' + customer.CustCnt_CellPhoneNum + '?call');
            $("a#cell").text(customer.CustCnt_CellPhoneNum);
            $("#openOrders").text(customer.Calculated_OpenOrder);
            $("#openAR").text('$' + customer.Calculated_OpenARValue);

            if (customer.Customer_CreditHold == false) {
                $("#onhold").text('N');
            } else {
                $("#onhold").text('Y');
            };

            $("#address1").text(customer.Customer_Address1);
            $("#address2").text(customer.Customer_City + ', ' + customer.Customer_State + ', ' + customer.Customer_Zip + ', ' + customer.Customer_Country);

            // get the chart data
            $.ajax({
                url: '../../api/e10/values',
                type: 'POST',
                data: JSON.stringify(request),
                contentType: 'application/json;charset=utf-8'
            }).done(function (chartData) {

                // show the years Sales Figures
                chartData.value.forEach(function (entry) {
                    chartXAxis.push(entry.Calculated_FiscalYear);

                    // divide the number by 1000 to make the chart more readable
                    var s = (parseFloat(entry.Calculated_TotalSales) / 1000).toFixed(2);
                    chartYAxis.push(s);
                });

                var data = {
                    labels: chartXAxis,
                    datasets: [{
                        data: chartYAxis
                    }]
                };

                var options = {
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: 'Sales by Year'
                    },
                    scales: {
                        yAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'x1000'
                            }
                        }]
                    }

                }

                try {

                    var ctx = $('#salesYTDChart')[0].getContext('2d');

                    var chartInstance = new Chart(ctx, {
                        type: 'bar',
                        data: data,
                        options: options
                    });

                } catch (err) {}

                var salesYTD = chartData.value[chartData.value.length - 1].Calculated_TotalSales;

                $("#salesYTD").text('$' + parseFloat(salesYTD).toFixed(2));

                // ensure the logon form is hidden and the profile section is visible.
                var logonForm = document.getElementById("logonForm");
                logonForm.setAttribute("class", "hiddenPage");
                var profile = document.getElementById("profile");
                profile.setAttribute("class", "displayedPage");

            }).always(function () {
                

            });

    }).fail(function (error) {

        // if the credentials are blank, or invalid or the token could not be validated then the error returned is 401 (Unauthorized)
        if (error.status = '401') {

            // show the logon form and hide the profile section
            var logonForm = document.getElementById("logonForm");
            logonForm.setAttribute("class", "displayedPage");
            var profile = document.getElementById("profile");
            profile.setAttribute("class", "hiddenPage");
        }

        // inform the user that the credentials were incorrect and prompt them to re-enter.
        //if (typeof fabric === "object") {
        //    if ('Toast' in fabric) {
        //        var component = new fabric['Toast'];
        //        component.showToast('Credentials Required', 'Please enter your Epicor 10 username and password. ');
        //    }
        //}

    }).always(function () {


    });
}

})();