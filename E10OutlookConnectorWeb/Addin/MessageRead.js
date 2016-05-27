/// <reference path="C:\Source\Repos\SandBox-RC-O365\Partner-PoC\Epicor\E10OutlookConnectorv1\E10OutlookConnectorWeb\Scripts/FabricUI/toast.js" />



(function () {
    "use strict";

    // The Office initialize function must be run each time a new page is loaded.
    Office.initialize = function (reason) {
        $(document).ready(function () {
            loadvalues();
        });
    };

    function loadvalues(event) {

        // get the context - in this the email address
        var item = Office.context.mailbox.item;
        var email = item.sender.emailAddress;
        var chartYTDData = new Array();

        // use the email address to lookup the contact in E10
        $.get('../../api/e10/?id=' + email, function (data, status) {

            $("#contact").text(data.Contact);
            $("#title").text(data.Title);
            $("#onhold").text(data.OnHold);
            $("#company").text(data.Company);
            $("a#email").attr('href', 'mailto:' + data.Email);
            $("a#email").text(data.Email);
            $("a#telephone").attr('href', 'skype:' + data.Telephone + '?call');
            $("a#telephone").text(data.Telephone);
            $("a#cell").attr('href', 'skype:' + data.Cell + '?call');
            $("a#cell").text(data.Cell);
            $("#openOrders").text(data.OpenOrders);
            $("#salesYtd").text(data.SalesYTD);
            $("#ar90days").text(data.AR90Days);
            $("#openAR").text(data.OpenAR);

            data.SalesByRegion.forEach(function (entry) {
                chartYTDData.push(entry);
            });

            var chartData = [
                {
                    value: chartYTDData[0].Value,
                    color: "#F7464A",
                    highlight: "#FF5A5E",
                    label: chartYTDData[0].Region
                },
                {
                    value: chartYTDData[1].Value,
                    color: "#46BFBD",
                    highlight: "#5AD3D1",
                    label: chartYTDData[1].Region
                },
                {
                    value: chartYTDData[2].Value,
                    color: "#FDB45C",
                    highlight: "#FFC870",
                    label: chartYTDData[2].Region
                },
                {
                    value: chartYTDData[3].Value,
                    color: "#FFBBFF",
                    highlight: "#FFC870",
                    label: chartYTDData[3].Region
                }
            ];

            var options = { animateScale: true, animateRotate: true, responsive: true };
            var DoughnutTextInsideChart = new Chart($('#salesYTDChart')[0].getContext('2d')).DoughnutTextInside(chartData, options);

            try {
                event.completed;
            }
            catch (err) { }

            // show a notification toast if the customer is on-hold
            if (data.OnHold == 'N') {
                if (typeof fabric === "object") {
                    if ('Toast' in fabric) {
                        var component = new fabric['Toast'];
                        component.showToast('Customer On-hold', 'Please be aware that this Customer is on-hold and all communications need to be handled carefully. ');
                    }
                }
            }


        });

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
    }


    // Helper function for displaying notifications
    function showNotification(header, content) {
        
    }
})();