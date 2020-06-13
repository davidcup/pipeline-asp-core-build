
def version = "1.0"
def slackChannel = "#alerts"

// define the slack notify build function
def notifyBuild(def buildStatus) {

    // set default of build status
    buildStatus =  buildStatus ?: 'STARTED'
    env.JOB_DISPLAYNAME = Jenkins.instance.getJob("${env.JOB_NAME}").displayName
    env.PREVIOUS_BUILD_RESULT = currentBuild.rawBuild.getPreviousBuild()?.getResult().toString()
    def colorMap = [ 'STARTED': '#F0FFFF', 'SUCCESS': '#008B00', 'FAILURE': '#FF0000' ]

    // Define messages contents
    def subject = "Pipeline: ${env.JOB_DISPLAYNAME} - #${env.BUILD_NUMBER} ${buildStatus}"
    def summary = "${subject} (${env.BUILD_URL})"
    def colorName = colorMap[buildStatus]

    // Send notifications only status changed.
    if ("${buildStatus}" != "${env.PREVIOUS_BUILD_RESULT}") {
        slackSend (color: colorName, message: summary, channel: slackChannel)
    }
}

def getBuildUser() {
    return currentBuild.rawBuild.getCause(Cause.UserIdCause).getUserId()
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
                notifyBuild('STARTED')
            }
        }
		
	    stage('\u278A Restore') {		
             steps {		
		     	sh(script: 'dotnet restore AspNetCoreApiDemo.sln', returnStdout: true)	
			 }
	    }
		  
		stage('\u278A Sonar Begin') {
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
		  
        stage('\u278A Build') {
            steps {
               sh 'dotnet build AspNetCoreApiDemo.sln -c Release'
            }
        }
		
		stage('\u278A Sonar End') {
			steps {					
				sh 'dotnet /opt/sonar-scanner/SonarScanner.MSBuild.dll end'				
			}
		}
    }
	
	post {
        success {
			script {
                BUILD_USER = getBuildUser()
            }
            notifyBuild(currentBuild.result)
        }
        failure {
             notifyBuild(currentBuild.result)
        }
        unstable {
             notifyBuild(currentBuild.result)
        }
    }
  
}