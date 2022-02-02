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
                margin: 1px;
                padding: 0;
                
            }

            p,h1,h2,h3,h4 {
                margin: 0;
                padding: 0;
            }

            .barcode_container {
                width: 29mm;
                height: 20mm;
                overflow: hidden;
            }

            @media only screen {
                .barcode_container {
                    display: inline-block;
                    margin: 10px;
                }
            }

            @media only print {
                .barcode_container {
                    page-break-after: always;
                    page-break-inside: avoid;
                }
            }

            .barcode_item {
                margin: 5px 0px 0px 10px;
            }

            .barcode_item .sku_img {
                height: 7mm;
                width: 25mm;
            }

            .barcode_item .sku_img img {
                height: 100%;
                width: 100%;
            }

            .barcode_item .sku,
            .barcode_item .item_name {
                text-align: center;
            }

            .barcode_item .sku {
                font-size: 7pt;
            }

            .barcode_item .item_name {
                font-size: 10pt;
            }
        </style>
        <script src="./plugins/JsBarcode.code128.min.js">//</script>
        <script src="./plugins/qrcode-generator.js">//</script>
        <script src="./plugins/qrcode.min.js">//</script>
        <script src="./code-gen.js">//</script>
    </head>
    <body>
        <xsl:for-each select="ArrayOfBarcodeXml/BarcodeXml">
            <div class="barcode_container">
                <div class="barcode_item">
                    <div class="sku_img">
                        <img src="./res/barcode.jpg" alt="" onload="load_sku(this);">
                            <xsl:attribute name="alt">
                                <xsl:value-of select="ItemNumber"/>
                            </xsl:attribute>
                        </img>  
                    </div>
                    <p class="sku"><xsl:value-of select="ItemNumber"/></p>
                    <p class="item_name"><xsl:value-of select="ItemName"/></p>
                </div>                    
            </div>
        </xsl:for-each> 
        <script>
            function load_sku(e) {
                var sku=e.getAttribute("alt");
                e.src=get_barcode(sku);
            }
        </script>
    </body>
</html>