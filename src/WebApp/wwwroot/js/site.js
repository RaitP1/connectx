(function () {
    "use strict";

    var pollInterval = null;
    var lastCurrentPlayer = null;

    function startPolling(gameId, mySlot) {
        if (pollInterval) return;

        lastCurrentPlayer = parseInt(
            document.querySelector("[data-current-player]")?.dataset.currentPlayer ?? "-1",
            10
        );

        pollInterval = setInterval(function () {
            fetch("/Games/Play?handler=Poll&gameId=" + encodeURIComponent(gameId))
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    if (data.currentPlayer !== lastCurrentPlayer || data.gameOver) {
                        location.reload();
                    }
                })
                .catch(function () { /* ignore transient errors */ });
        }, 2000);
    }

    var boardEl = document.getElementById("game-board");
    if (boardEl) {
        var gameId = boardEl.dataset.gameId;
        var mySlot = parseInt(boardEl.dataset.mySlot, 10);
        var currentPlayer = parseInt(boardEl.dataset.currentPlayer, 10);
        var gameOver = boardEl.dataset.gameOver === "true";

        if (!gameOver && currentPlayer !== mySlot) {
            startPolling(gameId, mySlot);
        }
    }
})();
