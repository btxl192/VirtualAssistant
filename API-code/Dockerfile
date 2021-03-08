FROM ubuntu:20.04
RUN apt update -y && \
    apt upgrade -y && \
    apt install python3 python3-pip -y && \
    useradd -m app && \
    mkdir -p /usr/src/app && \
    chown app:app -R /usr/src/app
USER app:app
WORKDIR /usr/src/app
COPY --chown=app:app ./requirements.txt /usr/src/app/requirements.txt
RUN pip3 install -r requirements.txt
COPY --chown=app:app . /usr/src/app
ENV USE_SSL=0
ENTRYPOINT [ "python3", "main.py" ]
