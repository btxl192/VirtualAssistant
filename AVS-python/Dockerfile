FROM ubuntu:18.04

RUN export DEBIAN_FRONTEND=noninteractive && \
		apt-get -y update && \
		apt-get install -y alsa-base alsa-utils curl gir1.2-gstreamer-1.0 git gstreamer1.0-plugins-bad gstreamer1.0-plugins-good gstreamer1.0-plugins-ugly libatlas-base-dev libpulse-dev mpg123 mpv portaudio19-dev pulseaudio python3-pip python-gi python-gst-1.0 python-pyaudio swig

ENV LANG=C.UTF-8

RUN mkdir /workspace && \
		useradd -m -u 1000 dev && \
		chown dev:dev /workspace
USER dev:dev
WORKDIR /workspace
COPY --chown=dev:dev . .
ENV PATH="/home/dev/.local/bin/:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"
RUN python3 -m pip install -r requirements.txt && \
		git clone --depth 1 https://github.com/Kitt-AI/snowboy.git snowboy_github && \
		cd snowboy_github && \
		python3 setup.py build && \
		python3 -m pip install . && \
		python3 -m pip install voice-engine

ENTRYPOINT [ "./entrypoint.sh" ]
