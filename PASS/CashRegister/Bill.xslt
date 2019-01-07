<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:template match="/">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <html>
      <body>
        <head>
          <title>Účtenka</title>
        </head>

        <style>
          #container {margin:0px auto;width:300px}
          table {width:100%;border-collapse:collapse;}
          #itemTable tr td {border-bottom:1px dashed black}
          #itemTable tr th {padding-top:5px}
          #headerTable {text-align:center;}
          #footerTable {text-align:center;}
        </style>

        <div id="container" style="">
          <header>

            <table id="headerTable">
              <tr>
                <td>
                  <b>
                    <xsl:value-of select="bill/companyName"/>
                  </b>
                </td>

              </tr>

              <xsl:if test="bill/companyAdress">
                <tr>
                  <td>
                    <xsl:value-of select="bill/companyAdress"/>
                  </td>
                </tr>
              </xsl:if>

              <xsl:if test="bill/companyCity or bill/companyPostalCode">
                <tr>
                  <td>
                    <xsl:value-of select="bill/companyCity"/>
                    <xsl:if test="bill/companyPostalCode and bill/companyCity">, </xsl:if>
                    <xsl:value-of select="bill/companyPostalCode"/>
                  </td>
                </tr>
              </xsl:if>

              <xsl:if test="bill/companyPhone">
                <tr>
                  <td>
                    Telefon: <xsl:value-of select="bill/companyPhone"/>
                  </td>
                </tr>
              </xsl:if>

              <xsl:if test="bill/companyWeb">
                <tr >
                  <td>
                    Internet: <xsl:value-of select="bill/companyWeb"/>
                  </td>
                </tr>

              </xsl:if>

            </table>
          </header>


          <table id="itemTable" style="margin-top:5px;border-top:1px dashed black">
            <tr>

              <th></th>
              <th style="text-align:right" > Kč </th>
            </tr>
            <xsl:for-each select="bill/product">
              <tr>
                <td>
                  <xsl:value-of select="name"/>
                  <xsl:text> </xsl:text>

                  <xsl:if test="priceForUnit = 'False'">
                    <xsl:value-of select="unitQuantity"/>
                  </xsl:if>

                  <xsl:if test="priceForUnit = 'True'">
                    <xsl:value-of select="quantity"/>
                  </xsl:if>

                  <xsl:value-of select="unit"/>


                  <xsl:if test="priceForUnit = 'False'">
                    <xsl:text disable-output-escaping='yes'>&lt;/br&gt;</xsl:text>
                    <span style="font-size:0.8em;font-style:italic">
                      <xsl:value-of select="quantity"/> X <xsl:value-of select="priceForSingleUnit"/> Kč
                    </span>                    

                  </xsl:if>
                </td>

                <td style="text-align:right">
                  <xsl:value-of select="totalPrice"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="vatType"/>
                </td>
              </tr>
            </xsl:for-each>

            <tr style="font-weight:bold;font-size:1.1em">
              <td >Celkem</td>
              <td colspan="3" style="text-align:right">
                <xsl:value-of select="bill/totalShoppingCartPrice"/>
              </td>
            </tr>
            <tr>
              <td >Přijato</td>
              <td colspan="3" style="text-align:right">
                <xsl:value-of select="bill/paid"/>
              </td>
            </tr>

            <tr>
              <td >Vráceno</td>
              <td colspan="3" style="text-align:right">
                <xsl:value-of select="bill/change"/>
              </td>

            </tr>


          </table>

          <table >
            <tr>
              <td>DPH Rekapitulace</td>
              <td></td>
            </tr>
            <tr>
              <td>Sazba</td>
              <td  >DPH</td>
              <td style="text-align:right">Celkem</td>
            </tr>
            <tr>
              <td>
                D<xsl:text> </xsl:text><xsl:value-of select="bill/D/@percentage"/>
              </td>
              <td >
                <xsl:value-of select="bill/D"/>
              </td>
              <td style="text-align:right">
                <xsl:value-of select="bill/D/@totalPrice"/>
              </td>

            </tr>
            <tr>
              <td>
                C<xsl:text> </xsl:text><xsl:value-of select="bill/C/@percentage"/>
              </td>
              <td >
                <xsl:value-of select="bill/C"/>
              </td>
              <td style="text-align:right">
                <xsl:value-of select="bill/C/@totalPrice"/>
              </td>
            </tr>
            <tr>
              <td>
                B<xsl:text> </xsl:text><xsl:value-of select="bill/B/@percentage"/>
              </td>
              <td >
                <xsl:value-of select="bill/B"/>
              </td>
              <td style="text-align:right">
                <xsl:value-of select="bill/B/@totalPrice"/>
              </td>
            </tr>
            <tr>
              <td>
                A<xsl:text> </xsl:text><xsl:value-of select="bill/A/@percentage"/>
              </td>
              <td >
                <xsl:value-of select="bill/A"/>
              </td>
              <td style="text-align:right">
                <xsl:value-of select="bill/A/@totalPrice"/>
              </td>
            </tr>
            <tr>
              <td>
                Celkem
              </td>
              <td>
                <xsl:value-of select="bill/vatSum"/>
              </td>
              <td style="text-align:right">
                <xsl:value-of select="bill/vatSumSingle"/>
              </td>

            </tr>

            <tr>
              <td >Obsluha</td>
              <td colspan="3" style="text-align:right">
                <xsl:value-of select="bill/staff"/>
              </td>
            </tr>
          </table>

          <!-- Patička -->
          <table id="footerTable"  style="margin-top:15px;border-top:1px dashed black">

            <tr >
              <td style="padding-top:5px">
                <xsl:value-of select="bill/time"/>
              </td>
            </tr>

            <xsl:if test="bill/billText">
              <tr>
                <td>
                  <xsl:value-of select="bill/billText"/>
                </td>
              </tr>
            </xsl:if>

          </table>

        </div>


      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
