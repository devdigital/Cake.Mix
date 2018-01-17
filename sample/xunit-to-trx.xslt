<?xml version="1.0" encoding="UTF-8" ?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output indent="yes" />
    <xsl:variable name="guidStub">
        <xsl:call-template name="testRunGuid">
            <xsl:with-param name="date" select="/assembly/@run-date"/>
            <xsl:with-param name="time" select="/assembly/@run-time"/>
        </xsl:call-template>
    </xsl:variable>

  <xsl:variable name="startDateTime">
    <xsl:value-of select="concat(/assemblies/assembly/@run-date, 'T', /assemblies/assembly/@run-time)"/>
  </xsl:variable>

  <!-- Set computer name and userName once so we can re-use. Hard coded until we can get the values passed -->
  <xsl:variable name="computerName">
    <xsl:value-of select="'TeamBuildServer'"/>
  </xsl:variable>
  <xsl:variable name="userName">
    <xsl:value-of select="'TeamBuildUser'"/>
  </xsl:variable>

  <xsl:template match="/">
        <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
            <!-- Collect totals from test that were run -->
            <xsl:variable name="totalTests">
              <xsl:value-of select="sum(//assemblies/assembly/collection/@total)" />
            </xsl:variable>
            <xsl:variable name="totalExecuted">
              <xsl:value-of select="sum(//assemblies/assembly/collection/@total) - sum(//assemblies/assembly/collection/@skipped)" />
            </xsl:variable>
            <xsl:variable name="totalNotExecuted">
              <xsl:value-of select="sum(//assemblies/assembly/collection/@skipped)" />
            </xsl:variable>
            <xsl:variable name="totalPassed">
              <xsl:value-of select="sum(//assemblies/assembly/collection/@passed)" />
            </xsl:variable>
            <xsl:variable name="totalFailed">
              <xsl:value-of select="sum(//assemblies/assembly/collection/@failed)" />
            </xsl:variable>
            <!-- Collect totals from test that were run -->
            <xsl:attribute name="id">
              <xsl:value-of select="concat($guidStub,'30db1d215203')"/>
            </xsl:attribute>
            <xsl:attribute name="runUser">
              <xsl:value-of select="concat($computerName,'',$userName)" />
            </xsl:attribute>
            <xsl:attribute name="name">
                <xsl:value-of select="concat($userName,'@',$computerName,' ',$startDateTime)"/>
            </xsl:attribute>
              <TestSettings name="Deftault" id="c136642c-2e64-4f99-9ec3-30db1d215203">
                  <Description>
                    Environment:<xsl:value-of select="/assemblies/assembly/@environment" />
                    Test framework:<xsl:value-of select="/assemblies/assembly/@test-framework" />
                  </Description>
              </TestSettings>
              <ResultSummary>
                <xsl:attribute name="outcome">
                    <xsl:choose>
                        <xsl:when test="/assemblies/assembly/collection/@failed &gt; 0">Failed</xsl:when>
                        <xsl:otherwise>Completed</xsl:otherwise>
                    </xsl:choose>
                </xsl:attribute>
                <Counters error="0" timeout="0" aborted="0" passedButRunAborted="0" notRunnable="0" disconnected="0" warning="0" completed="0" inProgress="0" pending="0">
                    <xsl:attribute name="total">
                      <xsl:value-of select="$totalTests"/>
                    </xsl:attribute>
                    <xsl:attribute name="executed">
                      <xsl:value-of select="$totalExecuted"/>
                    </xsl:attribute>
                    <xsl:attribute name="notExecuted">
                        <xsl:value-of select="$totalNotExecuted"/>
                    </xsl:attribute>
                    <xsl:attribute name="passed">
                        <xsl:value-of select="$totalPassed"/>
                    </xsl:attribute>
                    <xsl:attribute name="failed">
                        <xsl:value-of select="$totalFailed" />
                    </xsl:attribute>
                    <xsl:attribute name="inconclusive">
                        <xsl:value-of select="'0'"/>
                    </xsl:attribute>
                </Counters>
                <RunInfos />
            </ResultSummary>
        <Times>
        <xsl:attribute name="creation">
          <xsl:value-of select="$startDateTime"/>
        </xsl:attribute>
        <xsl:attribute name="queuing">
          <xsl:value-of select="$startDateTime"/>
        </xsl:attribute>
        <xsl:attribute name="start">
          <xsl:value-of select="$startDateTime"/>
        </xsl:attribute>
        <xsl:attribute name="finish">
          <xsl:value-of select="$startDateTime"/>
        </xsl:attribute>
      </Times>
            <TestDefinitions>
                <xsl:for-each select="//test">
                    <xsl:variable name="pos" select="position()" />
                    <UnitTest>
                        <xsl:attribute name="name">
                            <xsl:value-of select="@name"/>
                        </xsl:attribute>
                        <xsl:attribute name="storage">
                            <xsl:value-of select="concat(//environment/@cwd,/assembly/@name)"/>
                        </xsl:attribute>
                        <xsl:attribute name="id">
                            <xsl:call-template name="testIdGuid">
                                <xsl:with-param name="value" select="$pos"/>
                            </xsl:call-template>
                        </xsl:attribute>
                        <Css projectStructure="" iteration="" />
                        <xsl:if test="@description">
                            <Description><xsl:value-of select="@description" /></Description>
                        </xsl:if>
                        <Owners>
                            <Owner name="" />
                        </Owners>
                        <Execution>
                            <xsl:attribute name="id">
                                <xsl:call-template name="executionIdGuid">
                                    <xsl:with-param name="value" select="$pos"/>
                                </xsl:call-template>
                            </xsl:attribute>
                        </Execution>
                        <TestMethod adapterTypeName="Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" >
                            <xsl:attribute name="name">
                                <xsl:value-of select="@name"/>
                            </xsl:attribute>
                            <xsl:attribute name="codeBase">
                                <xsl:value-of select="concat(//environment/@cwd,/assembly/@name)"/>
                            </xsl:attribute>
                            <xsl:attribute name="className">
                                <xsl:variable name="testClassName">
                                    <xsl:call-template name="getTestClassName">
                                        <xsl:with-param name="type" select="@type"/>
                                    </xsl:call-template>
                                </xsl:variable>
                                <xsl:value-of select="$testClassName" />
                            </xsl:attribute>
                        </TestMethod>
                    </UnitTest>
                </xsl:for-each>
            </TestDefinitions>
            <TestLists>
                <TestList name="Results Not in a List" id="8c84fa94-04c1-424b-9868-57a2d4851a1d" />
                <TestList name="All Loaded Results" id="19431567-8539-422a-85d7-44ee4e166bda" />
            </TestLists>
            <TestEntries>
                <xsl:for-each select="//test">
                    <xsl:variable name="pos" select="position()" />
                    <TestEntry testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d">
                        <xsl:attribute name="testId">
                            <xsl:call-template name="testIdGuid">
                                <xsl:with-param name="value" select="$pos"/>
                            </xsl:call-template>
                        </xsl:attribute>
                        <xsl:attribute name="executionId">
                            <xsl:call-template name="executionIdGuid">
                                <xsl:with-param name="value" select="$pos"/>
                            </xsl:call-template>
                        </xsl:attribute>
                    </TestEntry>
                </xsl:for-each>
            </TestEntries>
            <Results>
                <xsl:for-each select="//test">
                    <xsl:variable name="pos" select="position()" />
                    <UnitTestResult testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d">
                      <xsl:attribute name="startTime">
                        <xsl:value-of select="$startDateTime"/>
                      </xsl:attribute>
                      <xsl:attribute name="endTime">
                        <xsl:value-of select="$startDateTime"/>
                      </xsl:attribute>
                        <xsl:attribute name="testName">
                            <xsl:value-of select="@name"/>
                        </xsl:attribute>
                        <xsl:attribute name="computerName">
                            <xsl:value-of select="$computerName"/>
                        </xsl:attribute>
                        <xsl:attribute name="duration">
                          <xsl:value-of select='format-number(@time, "00:00:00.0000000")' />
                        </xsl:attribute>
                        <xsl:attribute name="testId">
                            <xsl:call-template name="testIdGuid">
                                <xsl:with-param name="value" select="$pos"/>
                            </xsl:call-template>
                        </xsl:attribute>
                        <xsl:attribute name="executionId">
                            <xsl:call-template name="executionIdGuid">
                                <xsl:with-param name="value" select="$pos"/>
                            </xsl:call-template>
                        </xsl:attribute>
                        <xsl:attribute name="outcome">
                            <xsl:choose>
                <xsl:when test="@result='Pass'">
                  <xsl:value-of select="'Passed'"/>
                </xsl:when>
                <xsl:when test="@result='Fail'">
                  <xsl:value-of select="'Failed'"/>
                </xsl:when>
                <xsl:otherwise>
                <xsl:value-of select="'NotExecuted'"/>
                </xsl:otherwise>
                </xsl:choose>
                </xsl:attribute>
                        <Output>
                          <xsl:variable name="theoutput">
                            <xsl:value-of select="./output"/>
                          </xsl:variable>
                          <StdOut>
                            <xsl:value-of select="$theoutput"/>
                          </StdOut>
                          <xsl:for-each select="./failure">
                          <ErrorInfo>
                              <Message>
                                <xsl:value-of select="./message"/>
                              </Message>
                              <StackTrace>
                                <xsl:value-of select="./stack-trace"/>
                                ---
                                <xsl:value-of select="$theoutput"/>
                              </StackTrace>
                            </ErrorInfo>
                          </xsl:for-each>
                        </Output>
                    </UnitTestResult>
                </xsl:for-each>
            </Results>
        </TestRun>
    </xsl:template>

    <xsl:template name="substring-after-last">
        <xsl:param name="string" />
        <xsl:param name="delimiter" />
        <xsl:choose>
            <xsl:when test="contains($string, $delimiter)">
                <xsl:call-template name="substring-after-last">
                    <xsl:with-param name="string" select="substring-after($string, $delimiter)" />
                    <xsl:with-param name="delimiter" select="$delimiter" />
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$string" />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template name="getTestClassName">
      <!--Takes type in the form of Assembly.Class and returns Assembly.ClassName, Assembly -->
      <xsl:param name="type" />
        <xsl:value-of select="concat($type, ', ', substring-before($type, '.'))" />
    </xsl:template>
    <xsl:template name="testIdGuid">
        <xsl:param name="value" />
        <xsl:variable name="id">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$value"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:value-of select="concat($guidStub,substring(concat('000000000000', $id),string-length($id) + 1, 12))"/>
    </xsl:template>

    <xsl:template name="executionIdGuid">
        <xsl:param name="value" />
        <xsl:variable name="id">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$value"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:value-of select="concat($guidStub,substring(concat('000000000000', $id),string-length($id) + 1, 12))"/>
    </xsl:template>

    <xsl:template name="testRunGuid">
        <xsl:param name="date" />
        <xsl:param name="time" />
        <xsl:variable name="year">
            <xsl:value-of select="substring($date,1,4)"/>
        </xsl:variable>
        <xsl:variable name="month">
            <xsl:value-of select="substring($date,6,2)"/>
        </xsl:variable>
        <xsl:variable name="day">
            <xsl:value-of select="substring($date,9,2)"/>
        </xsl:variable>
        <xsl:variable name="hour">
            <xsl:value-of select="substring($time,1,2)"/>
        </xsl:variable>
        <xsl:variable name="minute">
            <xsl:value-of select="substring($time,4,2)"/>
        </xsl:variable>
        <xsl:variable name="second">
            <xsl:value-of select="substring($time,7,2)"/>
        </xsl:variable>
        <xsl:variable name="hexYear">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$year"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="hexMonth">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$month"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="hexDay">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$day"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="hexHour">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$hour"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="hexMinute">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$minute"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="hexSecond">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="$second"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="padYear">
            <xsl:value-of select="substring(concat('0000', $hexYear),string-length($hexYear) + 1, 4)"/>
        </xsl:variable>
        <xsl:variable name="padMonth">
            <xsl:value-of select="substring(concat('00', $hexMonth),string-length($hexMonth) + 1, 2)"/>
        </xsl:variable>
        <xsl:variable name="padDay">
            <xsl:value-of select="substring(concat('00', $hexDay),string-length($hexDay) + 1, 2)"/>
        </xsl:variable>
        <xsl:variable name="padHour">
            <xsl:value-of select="substring(concat('00', $hexHour),string-length($hexHour) + 1, 2)"/>
        </xsl:variable>
        <xsl:variable name="padMinute">
            <xsl:value-of select="substring(concat('00', $hexMinute),string-length($hexMinute) + 1, 2)"/>
        </xsl:variable>
        <xsl:variable name="padSecond">
            <xsl:value-of select="substring(concat('00', $hexSecond),string-length($hexSecond) + 1, 2)"/>
        </xsl:variable>
        <xsl:value-of select="concat($padYear,$padMonth,$padDay,'-',$padHour,$padMinute,'-',$padSecond,'00-91c4-')"/>
    </xsl:template>

    <xsl:variable name="hex_digits" select="'0123456789ABCDEF'" />

    <xsl:template name="dec_to_hex">
        <xsl:param name="value" />
        <xsl:if test="$value >= 16">
            <xsl:call-template name="dec_to_hex">
                <xsl:with-param name="value" select="floor($value div 16)" />
            </xsl:call-template>
        </xsl:if>
        <xsl:value-of select="substring($hex_digits, ($value mod 16) + 1, 1)" />
    </xsl:template>
    <xsl:template name="secondsToDuration">
        <xsl:param name="seconds" />
        <xsl:variable name="duration">
            <xsl:choose>
                <xsl:when test="$seconds">
                    <xsl:variable name="hours" select="floor($seconds div 3600)" />
                    <xsl:variable name="mins" select="'floor(($seconds – ($hours * 3600)) div 60)'" />
                    <xsl:variable name="secs" select="'floor($seconds – ($hours * 3600) – ($mins * 60))'" />
                    <xsl:variable name="frac" select="'substring($seconds – floor($seconds), 3, 7)'" />

                    <xsl:value-of select="substring(concat('00', $hours), string-length($hours) + 1, 2)" />
                    <xsl:text>:</xsl:text>
                    <xsl:value-of select="substring(concat('00', $mins) ,string-length($mins) + 1, 2)" />
                    <xsl:text>:</xsl:text>
                    <xsl:value-of select="substring(concat('00', $secs), string-length($secs) + 1, 2)" />
                    <xsl:if test="$frac > 0">
                      <xsl:value-of select="concat('.', $frac)" />
                    </xsl:if>
                </xsl:when>
                <xsl:otherwise>00:00:00.0000000</xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:value-of select="$duration" />
    </xsl:template>
</xsl:transform>
