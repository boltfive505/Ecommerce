<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE html>
<html xsl:version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <head>
        <meta http-equiv="X-UA-Compatible" content="IE=11"/>
    </head>
    <style>
        * {
            -moz-box-sizing: border-box; 
            -webkit-box-sizing: border-box; 
            box-sizing: border-box;
        }
        body {
            font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
            font-size: 12pt;
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
            .schedule_list_container {
                width: 148mm;
            }
        }

        .main_title {
            text-align: center;
        }

        .schedule_table th,
        .schedule_table td {
            padding: 3px 5px 3px 5px;
                vertical-align: middle;
                border: 1px solid #000000;
        }

        .schedule_table tbody tr .category {
            font-size: 10pt;
            color: gray;
            font-style: italic;
        }
    </style>
    <body>
        <div class="schedule_list_container">
            <h1 class="main_title mb-lg">SCHEDULE LIST</h1>
            <table class="schedule_table">
                <colgroup>
                    <col/>
                    <col style="width: 200px"/>
                    <col style="width: 55px;"/>
                </colgroup>
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Assigned To</th>
                        <th>Check</th>
                    </tr>
                </thead>
                <tbody>
                    <xsl:for-each select="ArrayOfScheduleXml/ScheduleXml">
                    <tr>
                        <td>
                            <p class="title"><xsl:value-of select="Title"/></p>
                            <p class="category"><xsl:value-of select="Category"/></p>
                        </td>
                        <td class="assigned_to"><xsl:value-of select="AssignedTo"/></td>
                        <td></td>
                    </tr>
                    </xsl:for-each>
                </tbody>
            </table>
        </div>
    </body>
</html>