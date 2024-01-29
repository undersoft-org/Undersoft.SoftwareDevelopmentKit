import '/_content/ServiceApplication.VideoPlayer/video.min.js';

var player = null;

export function loadPlayer(instance, id, options) {
    console.log('player id', id);
    player = videojs(id, options);

    player.ready(function () {
        console.log('player.ready');

        if (options.autoplay) {
            var promise = player.play();

            if (promise !== undefined) {
                promise.then(function () {
                    console.log('Autoplay started!');
                }).catch(function (error) {
                    console.log('Autoplay was prevented.', error);
                    instance.invokeMethodAsync('Logger', 'Autoplay was prevented.' + error);
                });
            }
        } else {
            player.poster(options.poster);
        }
        instance.invokeMethodAsync('GetInit');
    });

    return false;
}

export function setPoster(poster) {

    console.log(player.poster());
    player.poster(poster);
}

export function reloadPlayer(videoSource, type) {
    if (!player.paused) {
        player.pause();
    }


    console.log(player.currentSrc());

    player.src({ src: videoSource, type: type });
    player.load();
    player.play();
}

export function destroy(id) {
    if (undefined !== player && null !== player) {
        player = null;
        console.log('destroy');
    }
}