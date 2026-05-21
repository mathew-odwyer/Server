# Thanks Meseta, again :)

FROM ubuntu:20.04

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install --no-install-recommends --yes \
	curl \
	gnupg \
	ca-certificates \
	build-essential \
	openssh-server \
	clang \
	libssl-dev \
	libxrandr-dev \
	libxxf86vm-dev \
	libopenal-dev \
	libgl1-mesa-dev \
	libglu1-mesa-dev \
	zlib1g-dev \
	libcurl4-openssl-dev \
	mono-complete \
	zip \
	unzip \
	ffmpeg \
	rsync \
	git \
	libfuse2 \
	fuse \
	file \
	&& rm -rf /var/lib/apt/lists/*

# Steam runtimes
RUN mkdir /opt/steam-runtime/ && curl -s https://repo.steampowered.com/steamrt-images-scout/snapshots/latest-steam-client-general-availability/com.valvesoftware.SteamRuntime.Sdk-amd64,i386-scout-sysroot.tar.gz | tar -xzf - -C /opt/steam-runtime/

# Linuxdeploy
RUN curl -sL https://github.com/linuxdeploy/linuxdeploy/releases/download/continuous/linuxdeploy-x86_64.AppImage \
	-o linuxdeploy-x86_64.AppImage \
	# After download, ensure it exists
    && test -s linuxdeploy-x86_64.AppImage \
	# Install it at /usr/local/bin/linuxdeploy
    && install -m 0755 linuxdeploy-x86_64.AppImage /usr/local/bin/linuxdeploy \ 
	# Remove the downloaded file
    && rm linuxdeploy-x86_64.AppImage

# AppImage
RUN curl -sL https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage \
	-o appimagetool-x86_64.AppImage \
	# After download, ensure it exists
	&& test -s appimagetool-x86_64.AppImage \
	# Install it at /usr/local/bin/linuxdeploy
	&& install -m 0755 appimagetool-x86_64.AppImage /usr/local/bin/appimagetool \
	# Remove the downloaded file
	&& rm appimagetool-x86_64.AppImage 

# Set up SSH
RUN mkdir -p /run/sshd

# set up unprivilidged user
RUN useradd -ms /bin/bash builder && echo "builder:builder" | chpasswd

EXPOSE 22

CMD ["/usr/sbin/sshd", "-D", "-e"]