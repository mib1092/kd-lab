$(function () {
    $('[maxlength]').on('keyup', function () {
        var $this = $(this),
            maxLength = parseInt($this.attr('maxlength')),
            length = $this.val().length,
            charactersRemaining = maxLength - length,
            $wordCount = $this.closest('.tabContent').find('.wordCount'),
            $numberDisplay = $wordCount.find('.wordCountNumber');

        if (charactersRemaining <= 10) {
            $wordCount.addClass('wordCountLimit');
        }
        else {
            $wordCount.removeClass('wordCountLimit');
        }

        $numberDisplay.text(charactersRemaining);
    }).keyup();
})