/// <reference path="C:\Source\Repos\SandBox-RC-O365\Partner-PoC\Epicor\E10OutlookConnectorv1\E10OutlookConnectorWeb\Scripts/FabricUI/toast.js" />



(function () {
    "use strict";

    var request;

    // The Office initialize function must be run each time a new page is loaded.
    Office.initialize = function (reason) {
        $(document).ready(function () {

            $('#login').click(login);

            request = new Object();
            request.userName = "";
            request.password = "";
            request.context = "";
            request.token = "";

            loadvalues();
        });
    };

    function loadvalues() {

        if (request.token == "") {
            Office.context.mailbox.getUserIdentityTokenAsync(serviceCallback);
        } else {
            makeServiceRequest();
        }
    }

    function login() {
        var userName = document.getElementById("username");
        var password = document.getElementById("password");

        var logonForm = document.getElementById("logonForm");
        logonForm.setAttribute("class", "hiddenPage");

        var profile = document.getElementById("profile");
        profile.setAttribute("class", "displayedPage");

        request.userName = userName.value;
        request.password = password.value;

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

            var logonForm = document.getElementById("logonForm");
            logonForm.setAttribute("class", "hiddenPage");

            var profile = document.getElementById("profile");
            profile.setAttribute("class", "displayedPage");

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

            if (error.status = '401') {
                var logonForm = document.getElementById("logonForm");
                logonForm.setAttribute("class", "displayedPage");

                var profile = document.getElementById("profile");
                profile.setAttribute("class", "hiddenPage");
            }

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