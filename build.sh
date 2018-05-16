#!/bin/bash

if [ ! -f /build/dsyn ]; then 
	cd /build 
	mkdir edm_yocto 
	cd edm_yocto 
	git config --global user.email "you@email.com" 
	git config --global user.name "Bob Dole" 
	/home/bin/repo init -u "https://github.com/TechNexion/edm-yocto-bsp.git" -b krogoth_4.1.y_GA 
	/home/bin/repo sync -j8 
	touch /build/dsyn
fi

bitbake fsl-image-qt5
