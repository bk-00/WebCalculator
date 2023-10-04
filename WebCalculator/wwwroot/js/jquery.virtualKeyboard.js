;(function($){
    var virtualKeyboard = {
        template:
            '<div id="vkb-js-keyboard" class="vkb-css-hide">' +
                '<div class="vkb-css-background vkb-js-close"></div>' +
            
                '<div class="vkb-css-keyboard-box">' +
                    '<div class="vkb-css-input-box">' +
                        '<input type="text" id="vkb-js-input" class="vkb-css-input" placeholder="Enter your expression"/>' +
                    '</div>' +
                    
                    '<div class="vkb-css-button-box">' +
                        '<div class="vkb-css-button-line">' +
                            '<button type="button" id="vkb_8">< backspace</button>' +
                            '<button type="button"id="vkb_37"><</button>' +
                            '<button type="button"id="vkb_39">></button>' +
                        '</div>' +
            
                        '<div class="vkb-css-button-line">' +
                            '<button type="button">+</button>' +
                            '<button type="button">-</button>' +
                            '<button type="button">*</button>' +
                            '<button type="button">/</button>' +
                            '<button type="button">.</button>' +
                            '<button type="button">(</button>' +
                            '<button type="button">)</button>' +
                        '</div>' +
                        
                        '<div class="vkb-css-button-line">' +
                            '<button type="button">1</button>' +
                            '<button type="button">2</button>' +
                            '<button type="button">3</button>' +
                            '<button type="button">4</button>' +
                            '<button type="button">5</button>' +
                            '<button type="button">6</button>' +
                            '<button type="button">7</button>' +
                            '<button type="button">8</button>' +
                            '<button type="button">9</button>' +
                            '<button type="button">0</button>' +
                        '</div>' +
                            
                        '<div class="vkb-css-button-line">' +
                            '<button type="button" id="vkb_32"> </button>' +
                        '</div>' +
                    '</div>' +
                    
                    '<div class="vkb-css-footer-box">' +
                        '<button type="button" id="vkb_close">Cancel</button>' +
                        '<button type="button" id="vkb_success">OK</button>' +
                    '</div>' +
                '</div>' +
            '</div>',
        clickSumbolButton: function (e) {
            return function (e) {
                switch($(e.currentTarget).attr('id')) {
                    case "vkb_8":
                        this.pasteSymbolInPosition("");
                        break
                    case "vkb_32":
                        this.pasteSymbolInPosition(" ");
                        break
                    case "vkb_37":
                        if(this.input[0].selectionStart === this.input[0].selectionEnd)
                            this.setCursorPosition(this.input[0].selectionStart - 1);
                        else
                            this.setCursorPosition(this.input[0].selectionStart);
                        break
                    case "vkb_39":
                        if(this.input[0].selectionStart === this.input[0].selectionEnd)
                            this.setCursorPosition(this.input[0].selectionStart + 1);
                        else
                            this.setCursorPosition(this.input[0].selectionEnd);
                        break
                    case "vkb_close":
                        this.keyboard.hide();
                        this.input.val('');
                        break
                    case "vkb_success":
                        this.source.val('');
                        this.source.val(this.input.val());
                        this.keyboard.hide();
                        break
                    default:
                        this.pasteSymbolInPosition($(e.currentTarget).html());
                }
            }
        },
        show: function ($source) {
            this.source = $source;
            //console.log(this.source);
            this.input.val(this.source.val());
            this.keyboard.show();
        },
        pasteSymbolInPosition: function (symbol) {
                var selectionStart = (!symbol.length && this.input[0].selectionStart === this.input[0].selectionEnd && this.input[0].selectionStart !== 0) ? this.input[0].selectionStart - 1 : this.input[0].selectionStart;
                var parth1 = this.input.val().slice(0, selectionStart);
                var parth2 = this.input.val().slice(this.input[0].selectionEnd, this.input.val().length);
                this.input.val(parth1 + symbol + parth2);
                this.setCursorPosition(selectionStart + symbol.length);
                this.input.focus();
        },
        setCursorPosition: function (position) {
            if (this.input[0].setSelectionRange) {
                this.input[0].setSelectionRange(position, position);
            } else if (this.input[0].createTextRange) {
                var range = this.input[0].createTextRange();
                range.collapse(true);
                range.moveEnd('character', position);
                range.moveStart('character', position);
                range.select();
            }
        this.input.focus();
        }
    }

    function Vkb(element, options) {
        this.config = $.extend({}, options);
        this.element = element;
        this.init();
    }

    Vkb.prototype.init = function () {

        this.element.addClass(this.config.styleClassParentInput);

        $('body').append(virtualKeyboard.template);
        virtualKeyboard.keyboard = $('#vkb-js-keyboard');
        virtualKeyboard.input = $('#vkb-js-input');
        $('#vkb-js-keyboard button').on('click', $.proxy(virtualKeyboard.clickSumbolButton(this), virtualKeyboard));

        $('input[type=text]').on('click', function(e) {
            virtualKeyboard.show($(e.currentTarget));
            $('#vkb-js-input').val($(e.currentTarget).val()).focus();
        });

        $('textarea').on('click', function(e) {
            virtualKeyboard.show($(e.currentTarget));
            $('#vkb-js-input').val($(e.currentTarget).val()).focus();
        });
    }

    $.fn.vkb = function (options) {
        new Vkb(this, options);
        return this;
    }

})(jQuery);