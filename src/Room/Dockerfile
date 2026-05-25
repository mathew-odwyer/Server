# FROM ubuntu:20.04-minimal
FROM ubuntu:22.04

RUN apt-get update && apt-get install --no-install-recommends --yes \
  curl \
  ca-certificates \
  gpg \
  gpg-agent \
  dirmngr \
  wget \
  && rm -rf /var/lib/apt/lists/*

# needed to instal libssl1.0-dev and libssl1.1
RUN gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 40976EAF437D05B5 3B4FE6ACC0B21F32 && \
  gpg --export 40976EAF437D05B5 3B4FE6ACC0B21F32 > /etc/apt/trusted.gpg.d/security.ubuntu.com.gpg

# Needed for healthcheck
RUN wget -O /usr/local/bin/websocat https://github.com/vi/websocat/releases/download/v1.14.0/websocat.x86_64-unknown-linux-musl \
    && chmod +x /usr/local/bin/websocat

RUN echo "deb http://security.ubuntu.com/ubuntu xenial-security main" > /etc/apt/sources.list.d/xenial-security.list
RUN echo "deb http://security.ubuntu.com/ubuntu focal-security main" > /etc/apt/sources.list.d/focal-security.list

RUN apt-get update && apt-get install --no-install-recommends --yes \
  libxxf86vm1 \
  libgl1 \
  libssl1.1 \
  libxrandr2 \
  libglu1-mesa \
  libcurl4 \
  libopenal1 \
  xvfb \
  libssl1.0.0 \
  libcurl3-gnutls \
  lsb-release \
  unzip \
  libpulse0 \
  && rm -rf /var/lib/apt/lists/*

WORKDIR /root
COPY entrypoint.sh /root/entrypoint.sh
RUN chmod +x /root/entrypoint.sh

ENV DISPLAY=:0
ENTRYPOINT ["/root/entrypoint.sh"]

WORKDIR /game

# Copy the compiled room archive.
COPY output/Room.zip .

# Unzip the archive and then remove the archive.
RUN unzip Room.zip && rm Room.zip

# Ensure that the image can be ran.
RUN chmod +x ./Room

# Finally, run the app image.
CMD ["./Room"]