import groovy.json.JsonOutput
import groovy.json.JsonSlurper
import hudson.tasks.test.AbstractTestResultAction
import hudson.tasks.junit.CaseResult

def version = "1.0"
def slackNotificationChannel = "[CHANNEL_NAME]"
def author = ""
def message = ""
def testSummary = ""
def total = 0
def failed = 0
def skipped = 0
// define the slack notify build function

@NonCPS
def notifyBuild(def buildStatus) {

    def jobName = env.JOB_NAME
   
    // set default of build status
    buildStatus =  buildStatus ?: 'STARTED'
    env.JOB_DISPLAYNAME = jobName
    env.PREVIOUS_BUILD_RESULT = currentBuild.getPreviousBuild()?.getResult().toString()
    def colorMap = [ 'STARTED': '#F0FFFF', 'SUCCESS': '#008B00', 'FAILURE': '#FF0000' ]

    // Define messages contents
    def subject = "Pipeline: ${env.JOB_DISPLAYNAME} - #${env.BUILD_NUMBER} ${buildStatus}"
    def summary = "${subject} (${env.BUILD_URL})"
    def colorName = colorMap[buildStatus]

    // Send notifications only status changed.
    if ("${buildStatus}" != "${env.PREVIOUS_BUILD_RESULT}") {
        slackSend (color: colorName, message: summary, channel: "#alerts")
    }
}

@NonCPS
def getBuildUser() {
    return env.BUILD_USER_ID
}

@NonCPS
def sendNotifySlack(text, color) {
    def slackURL = 'https://hooks.slack.com/services/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'
    def jenkinsIcon = 'https://wiki.jenkins-ci.org/download/attachments/2916393/logo.png'
    def slackNotificationChannel = '#alerts'
    def slackBotName = getBuildUser()

    def payload = """
        {
            "attachments":[{
                "title": "Job: ${env.JOB_NAME}",
                "text":"${text}",
                "color":"${color}"
                }],
            "channel":"${slackNotificationChannel}",
            "username":"${slackBotName}",
            "icon_url":"${jenkinsIcon}"
        }
    """

    sh "curl -X POST --data-urlencode \'payload=${payload}\' ${slackURL}"
}

@NonCPS
def notifySlack(String buildResult){
   
    if ( buildResult == "SUCCESS" ) {
        sendNotifySlack("Job: ${env.JOB_NAME} with buildnumber ${env.BUILD_NUMBER} was successful", "good")
    }
    else if( buildResult == "FAILURE" ) { 
        sendNotifySlack("Job: ${env.JOB_NAME} with buildnumber ${env.BUILD_NUMBER} was failed", "danger")
    }
    else if( buildResult == "UNSTABLE" ) { 
        sendNotifySlack("Job: ${env.JOB_NAME} with buildnumber ${env.BUILD_NUMBER} was unstable", "warning")
    }
    else {
        sendNotifySlack("Job: ${env.JOB_NAME} with buildnumber ${env.BUILD_NUMBER} was unclear", "danger")
    }
}


pipeline {
	
	environment {
        BUILD_USER = ''
    }

    options { 
          disableConcurrentBuilds() 
	      buildDiscarder(logRotator(numToKeepStr: '5', artifactNumToKeepStr: '5'))
		  timestamps()
	}

    agent {
		node {
		  label 'dotnetcore'
		  customWorkspace '/home/david/jenkins/workspace/github-davidcup'
		}
    }	
  
    stages {
						
	    stage('\u278A Init') {		
             steps {
				script{		
                   notifySlack('STARTED')				 
				}
			 }
	    }
		  
		stage('\u278B Sonar Begin') {
			environment {
				sonarqubeScannerHome = tool 'SonarQubeScanner'
			}
			steps {	           
			    script{				
					withSonarQubeEnv('sonarqube') {
						sh "dotnet /opt/sonar-scanner/SonarScanner.MSBuild.dll begin /k:'aspnetcore-apidemo' /v:'${version}' /d:sonar.host.url='http://10.0.0.11:9095'"
					}					   
			   }
			}
        }  
		  
        stage('\u278C Build') {
            steps {
			   sh(script: 'dotnet restore AspNetCoreApiDemo.sln', returnStdout: true)
               sh 'dotnet build AspNetCoreApiDemo.sln -c Release'
            }
        }
		
		stage('\u278D Sonar End') {
			steps {					
				sh 'dotnet /opt/sonar-scanner/SonarScanner.MSBuild.dll end'				
			}
		}
    }
	
	post {
        success {
			script {                
				notifySlack(currentBuild.result)
            }
            
        }
        failure {
             script {                
				notifySlack(currentBuild.result)
            }
        }
        unstable {
             script {              
				notifySlack(currentBuild.result)
            }
        }
    }
  
}

