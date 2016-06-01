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

        $.ajax({
            url: '../../api/e10/',
            type: 'POST',
            data: JSON.stringify(request),
            contentType: 'application/json;charset=utf-8'
        }).done(function (data) {

            var customer = data.value[0];

            // ensure the logon form is hidden and the profile section is visible.
            var logonForm = document.getElementById("logonForm");
            logonForm.setAttribute("class", "hiddenPage");
            var profile = document.getElementById("profile");
            profile.setAttribute("class", "displayedPage");

            // apply the data from the API call to the elements within the HTML page.

            //$("#contact").text(data.Contact);
            //$("#title").text(data.Title);
            //$("#onhold").text(data.OnHold);
            //$("#company").text(data.Company);
            //$("a#email").attr('href', 'mailto:' + data.Email);
            //$("a#email").text(data.Email);
            //$("a#telephone").attr('href', 'skype:' + data.Telephone + '?call');
            //$("a#telephone").text(data.Telephone);
            //$("a#cell").attr('href', 'skype:' + data.Cell + '?call');
            //$("a#cell").text(data.Cell);
            //$("#openOrders").text(data.OpenOrders);
            //$("#salesYtd").text(data.SalesYTD);
            //$("#ar90days").text(data.AR90Days);
            //$("#openAR").text(data.OpenAR);

            //data.SalesByRegion.forEach(function (entry) {
            //    chartYTDData.push(entry);
            //});

            //var chartData = [
            //    {
            //        value: chartYTDData[0].Value,
            //        color: "#F7464A",
            //        highlight: "#FF5A5E",
            //        label: chartYTDData[0].Region
            //    },
            //    {
            //        value: chartYTDData[1].Value,
            //        color: "#46BFBD",
            //        highlight: "#5AD3D1",
            //        label: chartYTDData[1].Region
            //    },
            //    {
            //        value: chartYTDData[2].Value,
            //        color: "#FDB45C",
            //        highlight: "#FFC870",
            //        label: chartYTDData[2].Region
            //    },
            //    {
            //        value: chartYTDData[3].Value,
            //        color: "#FFBBFF",
            //        highlight: "#FFC870",
            //        label: chartYTDData[3].Region
            //    }
            //];

            //var options = { animateScale: true, animateRotate: true, responsive: true };
            //var DoughnutTextInsideChart = new Chart($('#salesYTDChart')[0].getContext('2d')).DoughnutTextInside(chartData, options);

            //// show a notification toast if the customer is on-hold
            //if (data.OnHold == 'Y') {
            //    if (typeof fabric === "object") {
            //        if ('Toast' in fabric) {
            //            var component = new fabric['Toast'];
            //            component.showToast('Customer On-hold', 'Please be aware that this Customer is on-hold and all communications need to be handled carefully. ');
            //        }
            //    }
            //}

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
            if (typeof fabric === "object") {
                if ('Toast' in fabric) {
                    var component = new fabric['Toast'];
                    component.showToast('Credentials Required', 'Please enter your Epicor 10 username and password. ');
                }
            }

        }).always(function () {

            Chart.types.Doughnut.extend({
                name: "DoughnutTextInside",
                showTooltip: function () {
                    this.chart.ctx.save();
                    Chart.types.Doughnut.prototype.showTooltip.apply(this, arguments);
                    this.chart.ctx.restore();
                },
                draw: function () {
                    Chart.types.Doughnut.prototype.draw.apply(this, arguments);

                    var width = this.chart.width,
                        height = this.chart.height;

                    var fontSize = (height / 114).toFixed(2);
                    this.chart.ctx.font = 0.5 + "em Verdana";
                    this.chart.ctx.textBaseline = "middle";

                    var text = "Sales by Region",
                        textX = Math.round((width - this.chart.ctx.measureText(text).width) / 2),
                        textY = height / 2;

                    this.chart.ctx.fillText(text, textX, textY);
                }
            });

        });
    }

})();