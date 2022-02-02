<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE html>
<html xsl:version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <head>
        <meta http-equiv="X-UA-Compatible" content="IE=11"/>
        <style>
            * {
                -moz-box-sizing: border-box; 
                -webkit-box-sizing: border-box; 
                box-sizing: border-box;
            }

            body {
                font-family: Calibri, sans-serif;
                font-size: 10pt;
                margin: 0;
                padding: 0;
            }

            p,h1,h2,h3,h4 {
                margin: 0;
            }

            table {
                table-layout: fixed;
                border-collapse: collapse;
                width: 100%;
            }

            table td,
            table th {
                padding: 0;
                margin: 0;
            }
    
            .mb-sm {
                margin-bottom: 10px;
            }
            .mb-lg {
                margin-bottom: 20px;
            }
            .mb-xlg {
                margin-bottom: 50px;
            }
    
            @media only screen {
                .ship_list_container {
                    min-width: 148mm;
                    max-width: 210mm;
                }
            }
    
            .main_title,
            .sub_title {
                text-align: center;
            }
    
            .subtotal_details_table tr p {
                font-weight: bold;
            }
    
            .subtotal_details_table tr .label {
                font-weight: normal;
                color: gray;
            }
    
            .ship_item_table th,
            .ship_item_table td {
                padding: 3px 5px 3px 5px;
                vertical-align: middle;
                border: 1px solid #000000;
            }
    
            .ship_item_table tbody tr img.sku_img {
                display: block;
                width: 85%;
                height: 30px;
                margin: 0 auto;
            }
    
            .ship_item_table tbody tr img.item_img {
                width: auto;
                height: 65px;
                float: left;
            }
    
            .ship_item_table tbody tr .description {
                font-size: 10pt;
                color: gray;
                font-style: italic;
            }
        </style>
        <script src="./plugins/JsBarcode.code128.min.js">//</script>
        <script src="./plugins/qrcode-generator.js">//</script>
        <script src="./plugins/qrcode.min.js">//</script>
        <script src="./code-gen.js">//</script>
    </head>
    <body>
        <div class="ship_list_container">
            <div class="mb-lg">
                <h1 class="main_title">On The Way Item List</h1>
            </div>
            <table class="subtotal_details_table mb-lg">
                <tr>
                    <td><p><span class="label">Shipping #: </span><xsl:value-of select="ShipListXml/Subtotal/ShipNumber"/></p></td>
                    <td><p><span class="label">Ship By: </span><xsl:value-of select="ShipListXml/Subtotal/ShipBy"/></p></td>
                </tr>
                <tr>
                    <td><p><span class="label">Packing Qty: </span><xsl:value-of select="ShipListXml/Subtotal/Quantity"/></p></td>
                    <td><p><span class="label">Status: </span><xsl:value-of select="ShipListXml/Subtotal/Status"/></p></td>
                </tr>
            </table>
            <table class="ship_item_table mb-lg">
                <colgroup>
                    <col style="width: 100px;"/>
                    <col/>
                    <col style="width: 60px;"/>
                    <col style="width: 50px;"/>
                </colgroup>
                <thead>
                    <tr>
                        <th>SKU</th>
                        <th>Item</th>
                        <th>Qty</th>
                        <th>Check</th>
                    </tr>
                </thead>
                <tbody>
                    <xsl:for-each select="ShipListXml/ItemList/ShipItem">
                        <tr>
                            <td>
                                <img class="sku_img" src="./res/barcode.jpg" alt="" onload="load_sku(this);">
                                    <xsl:attribute name="alt">
                                        <xsl:value-of select="ItemNumber"/>
                                    </xsl:attribute>
                                </img>
                                <p style="text-align: center"><xsl:value-of select="ItemNumber"/></p>
                            </td>
                            <td>
                                <img class="item_img">
                                    <xsl:attribute name="src">
                                        <xsl:value-of select="ImagePath"/>
                                    </xsl:attribute>
                                </img>
                                <p class="name"><xsl:value-of select="ItemName"/></p>
                                <p class="description"><xsl:value-of select="ItemDescription"/></p>
                            </td>
                            <td style="text-align:center;"><xsl:value-of select="Quantity"/></td>
                            <td></td>
                        </tr>
                    </xsl:for-each>
                </tbody>
            </table>
            <div class="check_container mb-lg">
                <p>After checking of Delivery Qty, input this items as Received Item. There should be P.O. at QB POS already. If not, report to management.</p>
                <p>Received &amp; Checked by: ________________________</p>
                <p>Fill Yellow Color at Shipping List Excel File: ________________________</p>
                <p>Recorded this Items on QB POS by: ________________________</p>
                <p>After recording, attach this with the Delivery Receipt, then file to <strong>Received Shipping DR (Delivery Receipt)</strong></p>
            </div>
            <div class="instructions_container">
                <p>All product from Korea should be recorded in to QB POS. This listing is already shipped and on the way to Philippines. So we need to create P.O for this products on QB POS before shipping arrival.</p>
                <p>When we receive shipping from korea, there should be a P.O in QB POS – so korea will know what product is already on the way from the QB POS.</p>
                <p>Print this page and record data into QB POS:</p>
                <ul>
                    <li>Create Purchase Order</li>
                    <li>Input Purchase Order Number ( Ask what data will be used for P.O number )</li>
                    <li>Scan barcode or type sku for the product listed in this document</li>
                </ul>
                <img src="res/qb po.png" alt="quickbooks pos purchase order" width="400" height="auto"/>
            </div>
        </div>
        <script>
            function load_sku(e) {
                var sku=e.getAttribute("alt");
                e.src=get_barcode(sku);
            }
        </script>
    </body>
</html>