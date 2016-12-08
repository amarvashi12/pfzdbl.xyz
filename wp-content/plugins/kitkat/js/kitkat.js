"use strict";
// todo: make jQuery reference '$' work inside this function!
(function () {
	var bak_button_val;

	$(document).ready(function () {
		// ready when you are!
	})
	// - fires on click of button
	.on('click', '.kitkat_btn', function () {
		var dobj_btn        = $(this),
			dobj_emailfield = $('.kitkat_email'),
			dobj_spinner    = $('.kitkat_spinner'),
			btn_action      = dobj_btn.attr('data-action'),
			obj             = {
				action : 'kitkat_' + btn_action,
				post_id: dobj_btn.parent().attr('data-id')
			},
			newAjaxReq;

		if ('reveal' === btn_action) {
			// memorizes button value
			bak_button_val = dobj_btn.val();

			obj.nonce  = dobj_btn.attr('data-noncebtn');
			newAjaxReq = // todo: fire AJAX call here. Hint: use already implemented function (see below) and pass parameters.

			newAjaxReq.done(function (res) {
				if ('success' === res.status && res.data.textforbutton && res.data.actionforbutton && res.data.nonceforemail) {

					// reveals text field for email address
					dobj_emailfield.kitkat_display();

					// sets new text for button
					dobj_btn.val(res.data.textforbutton);
					// sets action field for button
					dobj_btn.attr('data-action', res.data.actionforbutton);

					// sets nonce for email field
					dobj_emailfield.attr('data-nonceemail', res.data.nonceforemail);

				} else {
					console.log('An error occurred.');
				}
			}).fail(function (jqXHR, textStatus) {
				console.log('Request failed due to ' + textStatus);
			});
		} else if ('send' === btn_action) {
			// --- validates email and sends it

			// always remove error class of email field (fallback)
			dobj_emailfield.removeClass('kitkat_error');
			// always remove kitkat_active class (which hides the spinner)
			dobj_spinner.kitkat_hide();

			// if textfield for email is empty (error)
			if ( '' === dobj_emailfield.val() ) {
				// highlight textfield for email (error)
				console.log('email field is empty!');
				dobj_emailfield.addClass('kitkat_error');

			} else {
				// fire AJAX call to validate email address and send email
				obj.nonce  = dobj_emailfield.attr('data-nonceemail');
				obj.email  = dobj_emailfield.val();
				newAjaxReq = fire_ajax_call(obj);

				// overwrites button value with 'Sending ... '
				dobj_btn. // todo: set button value to 'Sending ... '.
				dobj_btn. // todo: disable button so it can't be clicked again

				dobj_spinner.kitkat_display();

				newAjaxReq.done(function (res) {
					if ('success' === res.status) {
						// - email was sent successfully

						// resets button
						dobj_btn.val(res.data.textforbutton);
						dobj_btn.attr('data-action', res.data.actionforbutton);
						dobj_btn.prop('disabled', false);
						dobj_btn.kitkat_hide();

						// clears nonce for sending
						dobj_emailfield.attr('data-nonceemail', res.data.nonceforemail);
						// clears email field for next use
						dobj_emailfield.val('');
						// hides email field
						dobj_emailfield.kitkat_hide();

						// removes spinner
						dobj_spinner.kitkat_hide();

						// todo: call function display_notification_for_secs() and pass on parameters 'Email sent!' etc. Hint: also look into CSS to display notification

					} else {

						console.log('unsuccessful');
						dobj_emailfield.addClass('kitkat_error');

						// resets button
						dobj_btn.val(bak_button_val);
					}
				}).fail(function (jqXHR, textStatus) {
					console.log('Send request failed. Due to ' + textStatus);

					// removes spinner
					dobj_spinner.kitkat_hide();
				});
			}
		}

	});


	/* --- general functions start here --- */
	/**
	 *
	 * @param obj
	 * @returns {*}
	 */
	function fire_ajax_call(obj) {

		return $.ajax({
			async   : true,
			type    : "POST",
			// todo: insert ajaxurl where we send this request to. Hint: Get it from DOM.
			url     : kitkat_params.ajaxurl,
			// todo: do not use caching. Hint: none.
			cache   : false,
			// todo: set timeout to 10 secs. Hint: none.
			timeout : 10000,
			// todo: request data of type "json". Hint: none.
			dataType: "json",
			data    : obj
		});
	}


	/**
	 *
	 * @param message
	 * @param duration
	 * @param status
	 * @param obj_toggle
	 */
	function display_notification_for_secs(message, duration, status, obj_toggle) {
		// status ....... either { success or danger } == > alert-success | alert-danger
		// strModalId ... HTML of the modal to be closed
		// - set default values
		var dobj_notification = $('.kitkat_notification');

		status   = typeof status !== 'undefined' ? 'alert-' + status : 'alert-danger';
		duration = typeof duration !== 'undefined' ? duration : 2000;

		// - resets notification bar and applies class
		dobj_notification.removeClass('alert-danger alert-success').addClass(status);

		// make sure to set the message!
		dobj_notification.html(message);

		// - animate the bar
		dobj_notification.fadeIn(function () {

			setTimeout(function () {
				dobj_notification.fadeOut(function () {
					// displays button (obj_toggle)
					obj_toggle.kitkat_display();
				});

			}, duration);
		});
	}


	$.fn.extend({
		// todo: write prototype functions to hide / display object. Hint: set use removeClass / addClass('kitkat_active').
	});
});
