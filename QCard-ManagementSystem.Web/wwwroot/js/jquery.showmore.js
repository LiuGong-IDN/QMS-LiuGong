/*
 * jQuery ShowMore plugin 0.0.1
 *
 * Copyright (c) 2013 (XMAESTRO)
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Date: Tue Nov 17 2013
 */

(function($) {

	$.fn.showmore = function(options) {

		//Attach current main variable to a variable for future use 
		var plugin = this;

		//Plugin defaults
		var settings = $.extend({

			visible : 5,
			childElement : "li",
			showMoreText : "<span>Show more</span>",
			showLessText : "<span>Show less</span>",
			showMoreClass : "show_more",
			showLessClass : "show_less"

		}, options);

		//Function for showing more
		plugin.show_less = function() {

			count = 0;

			plugin.find(settings.childElement).each(function() {

				if (count >= settings.visible) {

					$(this).hide();

				}

				count += 1;

			});

		}

		//Function for showing less
		plugin.show_more = function() {

			count = 0;

			plugin.find(settings.childElement).each(function() {

				$(this).show();

				count += 1;

			});

		}

		//Function called on plugin initialization
		plugin.init = function() {

			var current_show_more = jQuery.data(document.body,
					"current_show_more") == undefined ? 0 : jQuery.data(
					document.body, "current_show_more");

			jQuery.data(document.body, "current_show_more",
					parseInt(current_show_more) + 1);

			var new_show_more = jQuery.data(document.body, "current_show_more");

			plugin.show_less();

			plugin.after('<div class="kt-align-center kt-mt-10"><a class="btn btn-sm btn-label-twitter" href="javascript:;" id="'
					+ settings.showMoreClass + new_show_more + '" class="'
					+ settings.showMoreClass + '" >' + settings.showMoreText
					+ '</a></div>');
			plugin.after('<div class="kt-align-center kt-mt-10"><a class="btn btn-sm btn-label-twitter" href="javascript:;" id="'
					+ settings.showLessClass + new_show_more + '" class="'
					+ settings.showLessClass + '" style="display:none" >'
					+ settings.showLessText + '</a></div>');

			$("#" + settings.showMoreClass + new_show_more).click(function() {

				plugin.show_more();

				$("#" + settings.showMoreClass + new_show_more).hide();
				$("#" + settings.showLessClass + new_show_more).show();

			});

			$("#" + settings.showLessClass + new_show_more).click(function() {

				plugin.show_less();

				$("#" + settings.showLessClass + new_show_more).hide();
				$("#" + settings.showMoreClass + new_show_more).show();

			});

		}

		//Plugin initialization
		plugin.init();

	}

}(jQuery));
