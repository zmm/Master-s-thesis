FROM ubuntu:18.04

RUN apt-get update
RUN apt-get install -y python3.6 python3-pip

COPY requirements.txt /requirements.txt
RUN pip3 install -r requirements.txt

RUN apt-get install -y wget
# install google chrome
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list'
RUN apt-get -y update
RUN apt-get install -y google-chrome-stable
# RUN google-chrome --version

# install chromedriver
RUN apt-get install --no-install-recommends -yqq unzip
RUN apt-get install --no-install-recommends -y curl
RUN wget -O /tmp/chromedriver.zip http://chromedriver.storage.googleapis.com/`curl -sS chromedriver.storage.googleapis.com/LATEST_RELEASE`/chromedriver_linux64.zip
RUN unzip /tmp/chromedriver.zip chromedriver -d /usr/local/bin/

RUN pip3 install pytest
# set display port to avoid crash
ENV DISPLAY=:99

COPY . /