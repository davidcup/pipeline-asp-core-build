import groovy.json.JsonOutput
import groovy.json.JsonSlurper
import hudson.tasks.test.AbstractTestResultAction
import hudson.tasks.junit.CaseResult

def version = "1.0"
def slackNotificationChannel = "#alerts"
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
def notifySlack(text, channel, attachments) {
    def slackURL = 'https://hooks.slack.com/services/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'
    def jenkinsIcon = 'https://wiki.jenkins-ci.org/download/attachments/2916393/logo.png'

    def payload = JsonOutput.toJson([text: text,
        channel: channel,
        username: "Jenkins",
        icon_url: jenkinsIcon,
        attachments: attachments
    ])

    sh "curl -X POST --data-urlencode \'payload=${payload}\' ${slackURL}"
}

def isPublishingBranch = { ->
    return env.GIT_BRANCH == 'origin/master' || env.GIT_BRANCH =~ /release.+/
}

def isResultGoodForPublishing = { ->
    return currentBuild.result == null
}

def getGitAuthor = {
    def commit = sh(returnStdout: true, script: 'git rev-parse HEAD')
    author = sh(returnStdout: true, script: "git --no-pager show -s --format='%an' ${commit}").trim()
}

def getLastCommitMessage = {
    message = sh(returnStdout: true, script: 'git log -1 --pretty=%B').trim()
}

def populateGlobalVariables = {
    getLastCommitMessage()
    getGitAuthor()   
}

@NonCPS
def getBuildUser() {
    return env.BUILD_USER_ID
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
				   populateGlobalVariables()
                   notifyBuild('STARTED')				 
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
				script{
					def buildColor = currentBuild.result == null ? "good" : "warning"
					def buildStatus = currentBuild.result == null ? "Success" : currentBuild.result
					def jobName = "${env.JOB_NAME}"

					// Strip the branch name out of the job name (ex: "Job Name/branch1" -> "Job Name")
					jobName = jobName.getAt(0..(jobName.indexOf('/') - 1))
					
					
					sh(script: 'dotnet restore AspNetCoreApiDemo.sln', returnStdout: true)
					
					sh 'dotnet build AspNetCoreApiDemo.sln -c Release'
					
					notifySlack("", slackNotificationChannel, [
						[
							title: "${jobName}, Build #${env.BUILD_NUMBER}",
							title_link: "${env.BUILD_URL}",
							color: "${buildColor}",
							author_name: "${author}",
							text: "${buildStatus}\n${author}",
							fields: [
								[
									title: "Branch",
									value: "${env.GIT_BRANCH}",
									short: true
								],
								[
									title: "Last Commit",
									value: "${message}",
									short: false
								]
							]
						]
					])
				}
           
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
				notifyBuild(currentBuild.result)
            }
            
        }
        failure {
             script {                
				notifyBuild(currentBuild.result)
            }
        }
        unstable {
             script {              
				notifyBuild(currentBuild.result)
            }
        }
    }
  
}

